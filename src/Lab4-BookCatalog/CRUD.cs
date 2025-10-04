using LogSaveService;
using Microsoft.EntityFrameworkCore;

public static class CRUD
{
    public static void AddBookAsync()
    {
        try
        {
            Console.Write("ISBN: ");
            var isbn = Console.ReadLine() ?? "";
            Console.Write("Название: ");
            var title = Console.ReadLine() ?? "";
            Console.Write("Автор: ");
            var author = Console.ReadLine() ?? "";
            Console.Write("Год (оставьте пустым, если неизвестен): ");
            var yearStr = Console.ReadLine();
            Console.Write("Страницы (оставьте пустым, если неизвестно): ");
            var pagesStr = Console.ReadLine();

            var book = new Book
            {
                ISBN = isbn,
                Title = title,
                Author = author,
                Year = string.IsNullOrWhiteSpace(yearStr) ? null : int.Parse(yearStr),
                Pages = string.IsNullOrWhiteSpace(pagesStr) ? null : int.Parse(pagesStr),
            };

            using var context = new BooksContext();
            context.Books.Add(book);
            context.SaveChanges();
            SimpleLogger.Info("\nКнига добавлена!");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            SimpleLogger.Error(ex.Message);
        }
    }

    public static void UpdateBookAsync()
    {
        try
        {
            Console.Write("ISBN книги для обновления: ");
            var isbn = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(isbn)) return;

            using var context = new BooksContext();
            var book = context.Books.Find(isbn);
            if (book == null)
            {
                SimpleLogger.Info($"\nКнига - {isbn} не найдена.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Текущие данные: {book.Title} / {book.Author} / {book.Year} / {book.Pages}");
            Console.WriteLine("\nЧто обновить?");
            Console.WriteLine("1. Название");
            Console.WriteLine("2. Автор");
            Console.WriteLine("3. Год");
            Console.WriteLine("4. Страницы");
            Console.Write("Выбор: ");
            var field = Console.ReadLine();

            switch (field)
            {
                case "1":
                    Console.Write("Новое название: ");
                    book.Title = Console.ReadLine() ?? "";
                    break;
                case "2":
                    Console.Write("Новый автор: ");
                    book.Author = Console.ReadLine() ?? "";
                    break;
                case "3":
                    Console.Write("Новый год: ");
                    var y = Console.ReadLine();
                    book.Year = string.IsNullOrWhiteSpace(y) ? null : int.Parse(y);
                    break;
                case "4":
                    Console.Write("Новое количество страниц: ");
                    var p = Console.ReadLine();
                    book.Pages = string.IsNullOrWhiteSpace(p) ? null : int.Parse(p);
                    break;
                default:
                    Console.WriteLine("Отмена.");
                    Console.ReadKey();
                    return;
            }

            context.SaveChanges();
            SimpleLogger.Info("\nКнига обновлена!");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            SimpleLogger.Error(ex.Message);
        }
    }

    public static void DeleteBookAsync()
    {
        try
        {
            Console.Write("ISBN книги для удаления: ");
            var isbn = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(isbn)) return;

            using var context = new BooksContext();
            var book = context.Books.Find(isbn);
            if (book == null)
            {
                SimpleLogger.Info($"\nКнига - {isbn} не найдена.");
                Console.ReadKey();
                return;
            }

            context.Books.Remove(book);
            context.SaveChanges();
            SimpleLogger.Info("\nКнига удалена!");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            SimpleLogger.Error(ex.Message);
        }
    }

    public static void SearchBooksAsync()
    {
        try
        {
            Console.WriteLine("Поиск по:");
            Console.WriteLine("1. Автору");
            Console.WriteLine("2. Названию");
            Console.Write("Выбор: ");
            var mode = Console.ReadLine();
            Console.Write("Введите поисковый запрос: ");
            var query = Console.ReadLine() ?? "";

            using var context = new BooksContext();
            IQueryable<Book> booksQuery;

            switch (mode)
            {
                case "1":
                    booksQuery = context.Books.Where(b => EF.Functions.Like(b.Author, $"%{query}%"));
                    break;
                case "2":
                    booksQuery = context.Books.Where(b => EF.Functions.Like(b.Title, $"%{query}%"));
                    break;
                default:
                    SimpleLogger.Info("Неверный выбор.");
                    Console.ReadKey();
                    return;
            }

            var books = booksQuery.ToList();
            if (books.Count == 0)
            {
                SimpleLogger.Info("\nНичего не найдено.");
            }
            else
            {
                SimpleLogger.Info($"\nНайдено {books.Count} книг:");
                foreach (var b in books)
                {
                    Console.WriteLine($"{b.ISBN} | {b.Title} | {b.Author} | {b.Year?.ToString() ?? "—"} | {b.Pages?.ToString() ?? "—"}");
                }
            }
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            SimpleLogger.Error(ex.Message);
        }
    }
}