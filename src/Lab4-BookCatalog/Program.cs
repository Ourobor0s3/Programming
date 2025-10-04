using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using LogSaveService;
using Npgsql;
using NpgsqlTypes;

namespace Lab4_BookCatalog
{
    public static class Program
    {
        private const string ConnectionString = @"Host=localhost;Database=bookcatalog;Port=5433;User ID=postgres;Password=admin;";

        public static void Main(){}
        public static void Run1()
        {
            var books = new List<Book>();
            var successCount = 0;

            try
            {
                // 1. Парсинг CSV
                books = ProgramService.ParseCsv("../../../../Tasks/Lab4/books.csv");

                SimpleLogger.Info($"Прочитано {books.Count} записей из CSV.");
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
                    foreach (var book in books)
                    {
                        using var cmd = new NpgsqlCommand(
                            "insert into \"Books\" (\"ISBN\", \"Title\", \"Author\", \"Year\", \"Pages\") "
                            + "values (@ISBN, @Title, @Author, @Year, @Pages)",
                            connection, transaction);

                        cmd.Parameters.Add("@ISBN", NpgsqlDbType.Text).Value = book.ISBN ?? "";
                        cmd.Parameters.Add("@Title", NpgsqlDbType.Text).Value = book.Title ?? "";
                        cmd.Parameters.Add("@Author", NpgsqlDbType.Text).Value = book.Author ?? "";
                        cmd.Parameters.Add("@Year", NpgsqlDbType.Integer).Value = (object?)book.Year ?? DBNull.Value;
                        cmd.Parameters.Add("@Pages", NpgsqlDbType.Integer).Value = (object?)book.Pages ?? DBNull.Value;

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

            // 3. Экспорт в JSON и XML
            try
            {
                ProgramService.ExportToJson(books, "books.json");
                ProgramService.ExportToXml(books, "books.xml");
                SimpleLogger.Info("Экспорт в JSON и XML завершён успешно.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка экспорта: {ex.Message}");
            }
        }

        public static void Run2()
        {
            // Создаём БД и таблицу при первом запуске
            using (var context = new BooksContext())
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
                    case "1":
                        CRUD.AddBookAsync();
                        break;
                    case "2":
                        CRUD.UpdateBookAsync();
                        break;
                    case "3":
                        CRUD.DeleteBookAsync();
                        break;
                    case "4":
                        CRUD.SearchBooksAsync();
                        break;
                    case "5":
                        SyncService.Sync("update_db");
                        break;
                    case "0":
                        Console.WriteLine("Выход...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}