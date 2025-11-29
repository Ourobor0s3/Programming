using System.Text.Json;
using System.Text;
using LogSaveService;

public static class SyncService
{
    private const string JsonFile = "../../../../Tasks/Lab4/books.json";

    public static void Sync(string strategy)
    {
        using var context = new BooksContext();
        SimpleLogger.Info($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Начало синхронизации. Стратегия: {strategy}");

        // Загрузка из БД
        var dbBooks = (context.Books.ToList())
            .ToDictionary(b => b.ISBN, b => b);

        // Загрузка из JSON
        var jsonBooks = new Dictionary<string, Book>();
        if (File.Exists(JsonFile))
        {
            var json = File.ReadAllText(JsonFile);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
                jsonBooks = books.ToDictionary(b => b.ISBN, b => b);
            }
        }

        var allIsbns = new HashSet<string>(dbBooks.Keys.Concat(jsonBooks.Keys));
        var conflicts = new List<(string ISBN, Book? Db, Book? Json)>();

        foreach (var isbn in allIsbns)
        {
            var inDb = dbBooks.TryGetValue(isbn, out var dbBook);
            var inJson = jsonBooks.TryGetValue(isbn, out var jsonBook);

            switch (inDb)
            {
                case false when !inJson:
                    continue;
                case true when inJson:
                {
                    if (!BooksEqual(dbBook!, jsonBook!))
                    {
                        conflicts.Add((isbn, dbBook, jsonBook));
                    }

                    break;
                }
                default:
                    conflicts.Add((isbn, inDb ? dbBook : null, inJson ? jsonBook : null));
                    break;
            }
        }

        if (!conflicts.Any())
        {
            SimpleLogger.Info("Различий не обнаружено.");
        }
        else
        {
            SimpleLogger.Info($"Обнаружено {conflicts.Count} различий:");
            foreach (var (isbn, db, json) in conflicts)
            {
                if (db != null && json != null)
                    SimpleLogger.Error($"  [КОНФЛИКТ] ISBN={isbn}: данные различаются");
                else if (db != null)
                    SimpleLogger.Info($"  [ТОЛЬКО В БД] ISBN={isbn}");
                else
                    SimpleLogger.Info($"  [ТОЛЬКО В JSON] ISBN={isbn}");
            }

            // Применение стратегии
            foreach (var (isbn, db, json) in conflicts)
            {
                switch (strategy)
                {
                    case "update_db":
                        if (json != null)
                        {
                            if (db != null) context.Books.Remove(db);
                            context.Books.Add(json);
                            SimpleLogger.Info($"  → Обновлено в БД: {isbn}");
                        }
                        else if (db != null)
                        {
                            context.Books.Remove(db);
                            SimpleLogger.Info($"  → Удалено из БД: {isbn}");
                        }
                        break;

                    case "update_file":
                        // Просто будем перезаписывать json
                        break;

                    case "skip":
                        SimpleLogger.Info($"  → Пропущено: {isbn}");
                        break;
                }
            }

            switch (strategy)
            {
                case "update_db":
                    context.SaveChanges();
                    SimpleLogger.Info("Изменения применены к БД.");
                    break;
                case "update_file":
                {
                    // Обновляем JSON: берём все книги из БД
                    var allBooks = context.Books.ToList();
                    var json = JsonSerializer.Serialize(allBooks, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(JsonFile, json, Encoding.UTF8);
                    SimpleLogger.Info("Файл books.json обновлён из БД.");
                    break;
                }
            }
        }
    }

    private static bool BooksEqual(Book a, Book b)
    {
        return a.ISBN == b.ISBN &&
               a.Title == b.Title &&
               a.Author == b.Author &&
               a.Year == b.Year &&
               a.Pages == b.Pages;
    }
}