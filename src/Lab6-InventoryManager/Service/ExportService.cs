using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Lab6_InventoryManager.Entities;

namespace Lab6_InventoryManager.Service
{
    public static class ExportService
    {
        public static List<Product> ParseFile(string filePath)
        {
            var transactions = new List<Product>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var fields = ParseLine(line);
                try
                {
                    var transaction = new Product
                    {
                        ProductCode = fields[0],
                        Name = fields[1],
                        Price = decimal.Parse(fields[2], CultureInfo.InvariantCulture),
                        Stock = Convert.ToInt32(fields[3]),
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

        public static void ExportToJson<T>(List<T> list, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(list, options);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public static void ExportToXml<T>(List<T> list, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            serializer.Serialize(writer, list);
        }

        public static void GenerateHtmlReportFromXml(
            string xmlPath = "inventory.xml",
            string outputPath = "inventory.html")
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
                throw new ArgumentException("XML path cannot be empty.", nameof(xmlPath));

            if (!File.Exists(xmlPath))
                throw new FileNotFoundException($"XML file not found: {Path.GetFullPath(xmlPath)}", xmlPath);

            try
            {
                // 1) Загружаем XML
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);

                var rootNode = xmlDoc.DocumentElement;
                if (rootNode == null)
                    throw new InvalidOperationException("XML не содержит корневого элемента.");

                // 2) Определяем список полей автоматически по первой записи
                var firstNode = rootNode.ChildNodes.Cast<XmlNode>().FirstOrDefault();
                if (firstNode == null)
                    throw new InvalidOperationException("XML не содержит данных.");

                var columnNames = firstNode.ChildNodes
                    .Cast<XmlNode>()
                    .Select(n => n.Name)
                    .ToList();

                // 3) Генерируем HTML
                var sb = new StringBuilder();

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<meta charset=\"UTF-8\" />");
                sb.AppendLine("<title>Inventory Report</title>");
                sb.AppendLine("<style>");
                sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
                sb.AppendLine("th, td { border: 1px solid #ccc; padding: 6px; }");
                sb.AppendLine("th { background: #eee; }");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
                sb.AppendLine("<h2>Inventory Report</h2>");
                sb.AppendLine("<table>");

                // Заголовок
                sb.AppendLine("<tr>");
                foreach (var col in columnNames)
                    sb.AppendLine($"<th>{col}</th>");
                sb.AppendLine("</tr>");

                // Данные
                foreach (XmlNode itemNode in rootNode.ChildNodes)
                {
                    sb.AppendLine("<tr>");
                    foreach (var col in columnNames)
                    {
                        var val = itemNode[col]?.InnerText ?? "";
                        sb.AppendLine($"<td>{val}</td>");
                    }

                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");

                // 4) Сохраняем HTML
                var outDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка генерации HTML: {ex.Message}", ex);
            }
        }
    }
}