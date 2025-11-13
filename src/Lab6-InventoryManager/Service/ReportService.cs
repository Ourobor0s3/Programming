using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab6_InventoryManager.Service
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService(DataContext context)
        {
            _connectionString = context.Database.GetConnectionString()!;
        }

        // DTO для отчёта
        public record WarehouseStock(
            int WarehouseId,
            string WarehouseName,
            string ProductCode,
            string ProductName,
            int Quantity);

        public async Task<List<WarehouseStock>> GetWarehouseStocksAsync()
        {
            const string sql = @"
                SELECT
                    w.Id AS WarehouseId,
                    w.Name AS WarehouseName,
                    p.ProductCode,
                    p.Name AS ProductName,
                    COALESCE(SUM(
                        CASE WHEN sm.ToWarehouseId = w.Id THEN sm.Quantity ELSE 0 END -
                        CASE WHEN sm.FromWarehouseId = w.Id THEN sm.Quantity ELSE 0 END
                    ), 0) AS Quantity
                FROM Warehouses w
                CROSS JOIN Products p
                LEFT JOIN StockMovements sm ON sm.ProductCode = p.ProductCode
                GROUP BY w.Id, w.Name, p.ProductCode, p.Name
                HAVING COALESCE(SUM(
                        CASE WHEN sm.ToWarehouseId = w.Id THEN sm.Quantity ELSE 0 END -
                        CASE WHEN sm.FromWarehouseId = w.Id THEN sm.Quantity ELSE 0 END
                    ), 0) <> 0
                ORDER BY w.Id, p.ProductCode;";

            var result = new List<WarehouseStock>();

            await using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new WarehouseStock(
                    reader.GetInt32("WarehouseId"),
                    reader.GetString("WarehouseName"),
                    reader.GetString("ProductCode"),
                    reader.GetString("ProductName"),
                    reader.GetInt32("Quantity")));
            }

            return result;
        }
    }
}