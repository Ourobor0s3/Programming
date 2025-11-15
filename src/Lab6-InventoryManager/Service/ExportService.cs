using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
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

        public static string ExportToXml<T>(List<T> list, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var stringWriter = new StringWriter();
            using var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = false
            });

            serializer.Serialize(writer, list);
            return stringWriter.ToString();
        }

        // Генерация HTML через XSLT
        public static string GenerateHtmlReport(string xmlContent, string xsltPath = "inventory.xslt")
        {
            if (!File.Exists(xsltPath))
                throw new FileNotFoundException($"XSLT file not found: {xsltPath}");

            var xslt = new XslCompiledTransform();
            using var xsltReader = XmlReader.Create(xsltPath);
            xslt.Load(xsltReader);

            using var xmlReader = new StringReader(xmlContent);
            using var input = XmlReader.Create(xmlReader);

            using var ms = new MemoryStream();
            using var writer = XmlWriter.Create(ms, xslt.OutputSettings);
            xslt.Transform(input, writer);
            ms.Position = 0;

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}