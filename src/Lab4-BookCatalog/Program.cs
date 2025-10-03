using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using LogSaveService;
using Npgsql;

namespace Lab4_BookCatalog
{
    public static class Program
    {
        private const string ConnectionString = @"Host=localhost;Database=bookcatalog;Port=5433;User ID=postgres;Password=admin;";

        public static void Main()
        {
            var books = new List<Book>();
            var successCount = 0;

            try
            {
                // 1. Парсинг CSV
                books = ParseCsv("../../../../Tasks/Lab4/books.csv");

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

                foreach (var book in books)
                {
                    try
                    {
                        using var cmd = new NpgsqlCommand(
                            "INSERT INTO books (isbn, title, author, year, pages) VALUES (@ISBN, @Title, @Author, @Year, @Pages)",
                            connection);

                        cmd.Parameters.AddWithValue("@ISBN", book.ISBN ?? "");
                        cmd.Parameters.AddWithValue("@Title", book.Title ?? "");
                        cmd.Parameters.AddWithValue("@Author", book.Author ?? "");
                        cmd.Parameters.AddWithValue("@Year", book.Year);
                        cmd.Parameters.AddWithValue("@Pages", book.Pages);

                        cmd.ExecuteNonQuery();
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        SimpleLogger.Error($"Ошибка вставки книги ISBN={book.ISBN}: {ex.Message}");
                    }
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
                ExportToJson(books, "books.json");
                ExportToXml(books, "books.xml");
                SimpleLogger.Info("Экспорт в JSON и XML завершён успешно.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка экспорта: {ex.Message}");
            }
        }

        // --- Остальные методы без изменений (они не зависят от СУБД) ---

        static List<Book> ParseCsv(string filePath)
        {
            var books = new List<Book>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = ParseCsvLine(line);
                if (fields.Length != 5)
                {
                    throw new FormatException($"Неверное количество полей в строке {i + 1}: {line}");
                }

                try
                {
                    var book = new Book
                    {
                        ISBN = fields[0].Trim(),
                        Title = fields[1].Trim(),
                        Author = fields[2].Trim(),
                        Year = int.Parse(fields[3].Trim(), CultureInfo.InvariantCulture),
                        Pages = int.Parse(fields[4].Trim(), CultureInfo.InvariantCulture)
                    };
                    books.Add(book);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Ошибка парсинга строки {i + 1}: {ex.Message}. Строка: {line}");
                }
            }

            return books;
        }

        static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            fields.Add(current.ToString());
            return fields.ToArray();
        }

        static void ExportToJson(List<Book> books, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        static void ExportToXml(List<Book> books, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Book>));
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            serializer.Serialize(writer, books);
        }
    }
}