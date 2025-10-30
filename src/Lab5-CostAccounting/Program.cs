﻿using System.Globalization;
using LogSaveService;
using Npgsql;
using NpgsqlTypes;

namespace Lab5_CostAccounting
{
    public class Program
    {
        private const string ConnectionString = @"Host=localhost;Database=lab5-transaction;Port=5433;User ID=postgres;Password=admin;";

        public static void Main(){}
        public static void Run1()
        {
            List<Transaction> transactions;
            var successCount = 0;

            try
            {
                // 1. Парсинг Txt
                transactions = ExportService.ParseFile("../../../../Tasks/Lab5/transactions.txt");
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
                            $"insert into \"Transactions\" (\"Date\", \"Category\", \"Amount\", \"Note\") "
                                + "values (@Date, @Category, @Amount, @Note);",
                            connection, transaction);

                        cmd.Parameters.Add("@Date", NpgsqlDbType.Date).Value = trans.Date;
                        cmd.Parameters.Add("@Category", NpgsqlDbType.Text).Value = trans.Category ?? "";
                        cmd.Parameters.Add("@Amount", NpgsqlDbType.Numeric).Value = trans.Amount;
                        cmd.Parameters.Add("@Note", NpgsqlDbType.Text).Value = trans.Note ?? "";

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
                var report = transactions
                    .GroupBy(x => x.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Amount = g.Sum(x => x.Amount),
                    })
                    .Cast<object>()
                    .ToList();
                ExportService.ExportToJson(report, $"{DateTime.Now.ToShortDateString()}-summary.json");
                ExportService.ExportToXml(transactions, $"{DateTime.Now.ToShortDateString()}-summary.xml");
                SimpleLogger.Info("Экспорт в JSON и XML завершён успешно.");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error($"Ошибка экспорта: {ex.Message}");
            }
        }

        public static void Run2()
        {
            try
            {
                // Создаём БД и таблицу при первом запуске
                using (var context = new TransactionsContext())
                {
                    context.Database.EnsureCreatedAsync();
                }

                var prevTransactions = new List<Transaction>();
                while (true)
                {
                    Console.WriteLine("\n=== УПРАВЛЕНИЕ ТРАНЗАКЦИЯМИ ===");
                    Console.WriteLine("1. Отчет по категории за весь период");
                    Console.WriteLine("2. Получить фильтрованную выборку:");
                    Console.WriteLine("3. Экспорт предыдущей выборки в xml и json");
                    Console.WriteLine("0. Выход");
                    Console.Write("\nВыберите действие: ");

                    var choice = Console.ReadLine();
                    Console.WriteLine();

                    switch (choice)
                    {
                        case "1":
                            _ = TransactionService.GetJsonSumCategory();
                            break;
                        case "2":
                            prevTransactions = TransactionService.GetTransactions(
                                skip: 0,
                                take: 4,
                                sortDate: false,
                                sortCategory: false,
                                getDate: null,
                                getCategory: null);

                            break;
                        case "3":
                            if (prevTransactions.Count == 0)
                            {
                                Console.WriteLine("Выборка пустая, создание отчета невозможно!");
                                break;
                            }

                            ExportService.ExportToJson(prevTransactions, $"transactions_export.json");
                            ExportService.ExportToXml(prevTransactions, $"transactions_export.xml");
                            Console.WriteLine("Экспорт произошел успешно");
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}