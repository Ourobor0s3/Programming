using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace Lab5_CostAccounting
{
    public static class ExportService
    {
        public static List<Transaction> ParseFile(string filePath)
        {
            var transactions = new List<Transaction>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = ParseLine(line);
                try
                {
                    var transaction = new Transaction
                    {
                        Date = DateTime.Parse(fields[0]),
                        Category = fields[1],
                        Amount = decimal.Parse(fields[2]),
                        Note = fields[3],
                    };
                    transactions.Add(transaction);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Ошибка парсинга строки {i + 1}: {ex.Message}. Строка: {line}");
                }
            }

            return transactions;
        }

        private static string[] ParseLine(string line)
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

        public static void ExportToJson(List<Transaction> transaction, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(transaction, options);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public static void ExportToXml(List<Transaction> transaction, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Transaction>));
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            serializer.Serialize(writer, transaction);
        }
    }
}