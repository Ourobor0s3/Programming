using Lab6_InventoryManager.Entities;
using Lab6_InventoryManager.Service;
using LogSaveService;
using Npgsql;

namespace Lab6_InventoryManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }

        public static void Run1()
        {
            List<Product> products;
            var successCount = 0;

            try
            {
                // 1. Парсинг CSV
                products = ExportService.ParseFile("../../../../Tasks/Lab6/catalog.csv");
                SimpleLogger.Info($"Прочитано {products.Count} записей из CSV.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка при чтении CSV: {ex.Message}");
                return;
            }

            // 2. Подключение к БД и UPSERT
            try
            {
                using var connection = new NpgsqlConnection(DataContext.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    const string upsertSql = @"
                        INSERT INTO ""Products"" (""ProductCode"", ""Name"", ""Price"", ""Stock"")
                        VALUES (@ProductCode, @Name, @Price, @Stock)
                        ON CONFLICT (""ProductCode"") DO UPDATE
                        SET 
                            ""Name"" = EXCLUDED.""Name"",
                            ""Price"" = EXCLUDED.""Price"",
                            ""Stock"" = EXCLUDED.""Stock"";";

                    foreach (var product in products)
                    {
                        using var cmd = new NpgsqlCommand(upsertSql, connection, transaction);
                        cmd.Parameters.AddWithValue("@ProductCode", product.ProductCode ?? "");
                        cmd.Parameters.AddWithValue("@Name", product.Name ?? "");
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Stock", product.Stock);

                        cmd.ExecuteNonQuery();

                        // При UPSERT: 1 — вставка, 2 — обновление (но в PG обычно 1 всегда, т.к. ON CONFLICT не меняет affected rows)
                        successCount++;
                    }

                    transaction.Commit();
                    SimpleLogger.Info($"Успешно обработано {successCount} записей (вставлено/обновлено) в таблицу Products.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    SimpleLogger.Error($"Ошибка в транзакции при вставке в Products: {ex.Message}");
                    throw; // чтобы внешний catch знал, что произошла ошибка
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Критическая ошибка при работе с БД: {ex.Message}");
                return;
            }

            // 3. Генерация отчета
            try
            {
                ExportService.ExportToJson(products, "inventory.json");
                ExportService.ExportToXml(products, "inventory.xml");
                SimpleLogger.Info("Экспорт в JSON и XML завершён успешно.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка экспорта: {ex.Message}");
            }
        }

        public static void Run2()
        {
            var context = new DataContext();
            var stockService = new StockService(context);
            var reportService = new ReportService(context);
            var priceService = new PriceService(context);

            while (true)
            {
                Console.WriteLine("\n=== Inventory Manager ===");
                Console.WriteLine("1. Переместить товар");
                Console.WriteLine("2. Сгенерировать отчёт (XML/JSON/HTML)");
                Console.WriteLine("3. Создать склад");
                Console.WriteLine("4. Пополнить склад товаром из вне");
                Console.WriteLine("5. Подтянуть историю цен");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out var choice))
                    continue;

                switch (choice)
                {
                    case 1:
                        _ = HandleMoveAsync(stockService);
                        break;

                    case 2:
                        _ = HandleReportAsync(reportService);
                        break;

                    case 3:
                        Console.WriteLine("Введите название склада:");
                        var nameInput = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(nameInput))
                        {
                            Console.WriteLine("Название не может быть пустым.");
                            continue;
                        }

                        var warehouseId = stockService.CreateWarehouseAsync(nameInput.Trim()).Result;
                        SimpleLogger.Info($"Склад '{nameInput}' успешно создан. ID: {warehouseId}");
                        break;

                    case 4:
                        Console.Write("Код товара: ");
                        var code = Console.ReadLine();
                        Console.Write("ID склада: ");
                        if (!int.TryParse(Console.ReadLine(), out var wid))
                        {
                            Console.WriteLine("Неверный ID.");
                            continue;
                        }

                        Console.Write("Количество: ");
                        if (!int.TryParse(Console.ReadLine(), out var qty))
                        {
                            Console.WriteLine("Неверное количество.");
                            continue;
                        }

                        try
                        {
                            var success = stockService.ReplenishWarehouseAsync(code!, wid, qty).Result;
                            if (success)
                                Console.WriteLine("Пополнение успешно.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка: {ex.Message}");
                        }

                        break;

                    case 5:
                        Console.WriteLine("Импорт цен из supplier_feed.xml");
                        Console.Write("Путь к файлу (Enter — supplier_feed.xml): ");
                        var path = Console.ReadLine()?.Trim() ?? "supplier_feed.xml";
                        Console.Write("Порог изменения (%) [5]: ");
                        var threshInput = Console.ReadLine()?.Trim();
                        var threshold = threshInput != "" && decimal.TryParse(threshInput, out var t) ? t : 5.0m;

                        try
                        {
                            var updated = priceService.ImportPricesFromXmlAsync(path, threshold).Result;
                            Console.WriteLine(updated > 0
                                ? $"Обновлено {updated} цен"
                                : "ℹИзменений не обнаружено");
                        }
                        catch (Exception ex)
                        {
                            SimpleLogger.Error($"Ошибка: {ex.Message}");

                            // ex.InnerException для деталей
                        }

                        break;

                    case 0:
                        return;
                }
            }
        }

        private static async Task HandleMoveAsync(StockService svc)
        {
            try
            {
                SimpleLogger.Info("ProductCode: ");
                var pc = Console.ReadLine() ?? "";
                SimpleLogger.Info("FromWarehouseId: ");
                var from = int.Parse(Console.ReadLine() ?? "0");
                SimpleLogger.Info("ToWarehouseId: ");
                var to = int.Parse(Console.ReadLine() ?? "0");
                SimpleLogger.Info("Quantity: ");
                var qty = int.Parse(Console.ReadLine() ?? "0");

                var success = await svc.MoveStockAsync(pc, from, to, qty);
                SimpleLogger.Info(success ? "Перемещение успешно." : "Не удалось.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка: {ex.Message}");
            }
        }

        private static async Task HandleReportAsync(ReportService svc)
        {
            try
            {
                var data = await svc.GetWarehouseStocksAsync();

                ExportService.ExportToJson(data, "inventory.json");
                ExportService.ExportToXml(data, "inventory.xml");
                ExportService.GenerateHtmlReport(data, "inventory.html");
                SimpleLogger.Info("Отчёты сохранены: report.xml, report.json, report.html");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка генерации: {ex.Message}");
                if (ex.InnerException != null)
                    SimpleLogger.Error($"Inner: {ex.InnerException.Message}");
            }
        }
    }
}