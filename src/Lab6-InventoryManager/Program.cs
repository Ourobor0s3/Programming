using Lab6_InventoryManager.Entities;
using Lab6_InventoryManager.Service;
using LogSaveService;
using Npgsql;
using NpgsqlTypes;

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
    }
}