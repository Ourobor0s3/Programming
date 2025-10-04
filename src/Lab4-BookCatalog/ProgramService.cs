using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace Lab4_BookCatalog
{
    public class ProgramService
    {
        public static List<Book> ParseCsv(string filePath)
        {
            var books = new List<Book>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (var i = 1; i < lines.Length; i++)
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

        public static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var inQuotes = false;
            var current = new StringBuilder();

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                switch (c)
                {
                    case '"' when inQuotes && i + 1 < line.Length && line[i + 1] == '"':
                        current.Append('"');
                        i++;
                        break;
                    case '"':
                        inQuotes = !inQuotes;
                        break;
                    case ',' when !inQuotes:
                        fields.Add(current.ToString());
                        current.Clear();
                        break;
                    default:
                        current.Append(c);
                        break;
                }
            }

            fields.Add(current.ToString());
            return fields.ToArray();
        }

        public static void ExportToJson(List<Book> books, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public static void ExportToXml(List<Book> books, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Book>));
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            serializer.Serialize(writer, books);
        }
    }
}