using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static System.Console;

namespace Lab5_CostAccounting
{
    public static class TransactionService
    {
        public static List<Transaction> GetTransactions(
            int skip = 0,
            int take = 4,
            bool sortDate = false,
            bool sortCategory = false,
            DateTime? getDate = null,
            string? getCategory = null)
        {
            using var context = new TransactionsContext();
            var transactions = context.Transactions;

            if (getDate != null)
                transactions.Where(x => x.Date.Date.Equals(getDate.Value.Date));

            if (getCategory != null)
                transactions.Where(x => x.Category!.Equals(getCategory));

            if (sortDate)
                transactions.OrderBy(x => x.Date);

            if (sortCategory)
                transactions.OrderBy(x => x.Category);

            var res = transactions
                .Skip(skip)
                .Take(take)
                .ToListAsync().Result;
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(res, options);
            WriteLine($"Фильтрованный отчет: {json}");
            return res;
        }

        public static async Task GetJsonSumCategory()
        {
            try
            {
                await using var context = new TransactionsContext();
                var transactions = context.Transactions
                    .GroupBy(x => x.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Amount = g.Sum(x => x.Amount),
                    })
                    .Cast<object>()
                    .ToListAsync().Result;
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(transactions, options);
                WriteLine($"Отчет: {json}");
            }
            catch (Exception ex)
            {
                WriteLine(ex);
            }

        }
    }
}