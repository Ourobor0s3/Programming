using System.Text.Json;
using LogSaveService;
using Microsoft.EntityFrameworkCore;

namespace Lab5_CostAccounting.Service
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
            IQueryable<Transaction> query = context.Transactions;

            if (getDate.HasValue)
            {
                var date = getDate.Value.Date;
                var nextDay = date.AddDays(1);
                query = query.Where(x => x.Date >= date && x.Date < nextDay);
            }

            if (!string.IsNullOrWhiteSpace(getCategory))
            {
                var cat = getCategory.Trim();
                query = query.Where(x => x.Category != null &&
                                         x.Category.Equals(cat, StringComparison.OrdinalIgnoreCase));
            }

            switch (sortDate)
            {
                case true when sortCategory:
                    query = query.OrderBy(x => x.Date).ThenBy(x => x.Category);
                    break;
                case true:
                    query = query.OrderBy(x => x.Date);
                    break;
                default:
                {
                    if (sortCategory)
                    {
                        query = query.OrderBy(x => x.Category);
                    }

                    break;
                }
            }

            var res = query
                .Skip(skip)
                .Take(take)
                .ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(res, options);
            SimpleLogger.Info($"Фильтрованный отчет: {json}");

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
                SimpleLogger.Info($"Отчет: {json}");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error(ex.Message);
            }

        }
    }
}