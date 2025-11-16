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

        public static void ExportToXml<T>(List<T> list, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            serializer.Serialize(writer, list);
        }

        public static void GenerateHtmlReport<T>(List<T> stocks, string xsltPath = "inventory.xslt")
        {
            if (stocks == null)
                throw new ArgumentNullException(nameof(stocks));

            if (string.IsNullOrWhiteSpace(xsltPath))
                throw new ArgumentException("XSLT path cannot be null or empty.", nameof(xsltPath));

            if (!File.Exists(xsltPath))
                throw new FileNotFoundException($"XSLT file not found: {Path.GetFullPath(xsltPath)}", xsltPath);

            try
            {
                // 1. Сериализуем List<WarehouseStock> в XML-строку
                var xmlSerializer = new XmlSerializer(typeof(List<T>));
                var xmlContent = new StringWriter();
                xmlSerializer.Serialize(xmlContent, stocks);
                var xmlString = xmlContent.ToString();

                // 2. Загружаем XSLT
                var xslt = new XslCompiledTransform();
                var settings = new XsltSettings { EnableDocumentFunction = false, EnableScript = false };
                xslt.Load(xsltPath, settings, new XmlUrlResolver());

                // 3. Настройки вывода (гарантируем UTF-8)
                var outputSettings = xslt.OutputSettings?.Clone() ?? new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true,
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\n",
                    ConformanceLevel = ConformanceLevel.Auto
                };
                outputSettings.Encoding = Encoding.UTF8;

                // 4. Трансформация
                using var xmlReader = XmlReader.Create(new StringReader(xmlString));
                using var ms = new MemoryStream();
                using var writer = XmlWriter.Create(ms, outputSettings);

                xslt.Transform(xmlReader, writer);
                writer.Flush();

                ms.Position = 0;
                new StreamReader(ms, Encoding.UTF8).ReadToEnd();
                return;
            }
            catch (XsltException ex)
            {
                throw new InvalidOperationException($"Ошибка в XSLT-файле '{xsltPath}': {ex.Message}", ex);
            }
            catch (InvalidOperationException ex) when (ex.InnerException is XmlException)
            {
                throw new InvalidOperationException($"Ошибка сериализации данных в XML: {ex.InnerException.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not ArgumentNullException && ex is not FileNotFoundException)
            {
                throw new InvalidOperationException($"Не удалось сгенерировать HTML-отчёт: {ex.Message}", ex);
            }
        }
    }
}