using System.Xml.Linq;
using Lab6_InventoryManager.Entities;
using LogSaveService;
using Microsoft.EntityFrameworkCore;

namespace Lab6_InventoryManager.Service
{
    public class PriceService
    {
        private readonly DataContext _context;

        public PriceService(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Импортирует цены из XML-файла и обновляет историю при значимых изменениях.
        /// </summary>
        public Task<int> ImportPricesFromXmlAsync(
            string filePath,
            decimal thresholdPercent = 5.0m,
            string? reason = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required.", nameof(filePath));
            if (thresholdPercent < 0)
                throw new ArgumentException("Threshold must be non-negative.", nameof(thresholdPercent));

            reason ??= "Supplier feed import";

            // Парсинг XML (идемпотентно: одинаковый файл → одинаковые данные)
            var updates = ParsePriceFeedXml(filePath);

            SimpleLogger.Info($"Загружено {updates.Count} записей цен из {Path.GetFileName(filePath)}");

            return _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var updatedCount = 0;

                    foreach (var entry in updates)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.ProductCode == entry.ProductCode, ct);

                        if (product == null)
                        {
                            SimpleLogger.Warn($"Продукт {entry.ProductCode} из фида не найден в системе");
                            continue;
                        }

                        // Расчёт относительного изменения
                        var oldPrice = product.Price;
                        var newPrice = entry.NewPrice;

                        // Проверка: изменилась ли цена вообще?
                        if (oldPrice == newPrice)
                        {
                            SimpleLogger.Info($"Цена продукта {entry.ProductCode} не изменилась: {oldPrice}");
                            continue;
                        }

                        // Проверка порога: |Δ%| > threshold?
                        var changePercent = Math.Abs((newPrice - oldPrice) / (oldPrice == 0 ? 1m : oldPrice)) * 100;

                        if (changePercent >= thresholdPercent || oldPrice == 0)
                        {
                            // Идемпотентность: проверим, не было ли уже такой записи в этот день?
                            // (чтобы повторный импорт за один день не дублировал)
                            var alreadyRecorded = await _context.PriceHistory
                                .AnyAsync(
                                    ph => ph.ProductCode == entry.ProductCode
                                             && ph.OldPrice == oldPrice
                                             && ph.NewPrice == newPrice
                                             && ph.ChangedAt.Date == DateTime.UtcNow.Date, ct);

                            if (!alreadyRecorded)
                            {
                                _context.PriceHistory.Add(new PriceHistory
                                {
                                    ProductCode = entry.ProductCode,
                                    OldPrice = oldPrice,
                                    NewPrice = newPrice,
                                    ChangedAt = DateTime.UtcNow,
                                    Reason = reason
                                });

                                // Обновляем цену в продукте
                                product.Price = newPrice;
                                _context.Products.Update(product);

                                SimpleLogger.Info(
                                    $"Цена продукта {entry.ProductCode} изменена: {oldPrice} → {newPrice} ({changePercent:F2}%, порог {thresholdPercent}%)");

                                updatedCount++;
                            }
                            else
                            {
                                SimpleLogger.Info($"Изменение цены для {entry.ProductCode} уже записано сегодня — пропускаем");
                            }
                        }
                        else
                        {
                            SimpleLogger.Info(
                                $"Изменение цены продукта {entry.ProductCode} ({oldPrice} → {newPrice}) меньше порога ({changePercent:F2}% < {thresholdPercent}%) — не записываем в историю");
                        }
                    }

                    if (updatedCount > 0)
                    {
                        await _context.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);
                        SimpleLogger.Info($"Успешно обновлено {updatedCount} цен");
                    }
                    else
                    {
                        await transaction.RollbackAsync(ct);
                        SimpleLogger.Info("Изменений цен не обнаружено");
                    }

                    return updatedCount;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
        }

        private List<PriceUpdateEntry> ParsePriceFeedXml(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            try
            {
                var doc = XDocument.Load(filePath);
                var entries = doc.Root?.Elements("Product")
                    .Select(el => new PriceUpdateEntry
                    {
                        ProductCode = el.Element("ProductCode")?.Value.Trim() ?? "",
                        NewPrice = decimal.Parse(
                            el.Element("Price")?.Value ?? "0",
                            System.Globalization.CultureInfo.InvariantCulture)
                    })
                    .Where(e => !string.IsNullOrWhiteSpace(e.ProductCode))
                    .ToList() ?? new List<PriceUpdateEntry>();

                if (!entries.Any())
                    throw new InvalidOperationException("Файл не содержит валидных записей <Product>");

                return entries;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка парсинга XML-файла '{filePath}': {ex.Message}", ex);
            }
        }
    }
}