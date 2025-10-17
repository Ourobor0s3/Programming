using LogSaveService;
using Npgsql;
using NpgsqlTypes;

namespace Lab5_CostAccounting
{
    public class Program
    {
        private const string ConnectionString = @"Host=localhost;Database=lab5-transaction;Port=5433;User ID=postgres;Password=admin;";

        public static void Run1()
        {
            var transactions = new List<Transaction>();
            var successCount = 0;

            try
            {
                // 1. Парсинг Txt
                transactions = ExportService.ParseFile("../../../../Tasks/Lab5/transaction.txt");

                SimpleLogger.Info($"Прочитано {transactions.Count} записей из CSV.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка при чтении CSV: {ex.Message}");
                return;
            }

            // 2. Подключение к БД и вставка
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    foreach (var trans in transactions)
                    {
                        using var cmd = new NpgsqlCommand(
                            "insert into \"Transaction\" (\"ISBN\", \"Title\", \"Author\", \"Year\", \"Pages\") "
                            + "values (@ISBN, @Title, @Author, @Year, @Pages)",
                            connection, transaction);

                        // cmd.Parameters.Add("@ISBN", NpgsqlDbType.Text).Value = trans.ISBN ?? "";

                        cmd.ExecuteNonQuery();
                        successCount++;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    SimpleLogger.Error($"Ошибка транзакции: {ex.Message}");
                }

                SimpleLogger.Info($"Успешно вставлено {successCount} записей в БД.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Критическая ошибка при работе с БД: {ex.Message}");
                return;
            }

            // 3. Генерация отчета
            try
            {
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка экспорта: {ex.Message}");
            }
        }

        public static void Run2()
        {
            // Создаём БД и таблицу при первом запуске
            using (var context = new TransactionsContext())
            {
                context.Database.EnsureCreatedAsync();
            }

            while (true)
            {
                Console.WriteLine("=== УПРАВЛЕНИЕ КАТАЛОГОМ КНИГ ===");
                Console.WriteLine("1. Добавить книгу");
                Console.WriteLine("2. Обновить книгу");
                Console.WriteLine("3. Удалить книгу по ISBN");
                Console.WriteLine("4. Поиск по автору или названию");
                Console.WriteLine("5. Синхронизация с books.json");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}