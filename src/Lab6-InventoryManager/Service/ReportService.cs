using System.Data;
using Lab6_InventoryManager.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Lab6_InventoryManager.Service
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService(DataContext context)
        {
            _connectionString = context.Database.GetConnectionString()!;
        }

        public async Task<List<WarehouseStock>> GetWarehouseStocksAsync()
        {
            const string sql = @"
                SELECT
                    w.""Id"" AS WarehouseId,
                    w.""Name"" AS WarehouseName,
                    p.""ProductCode"",
                    p.""Name"" AS ProductName,
                    COALESCE(SUM(
                                     CASE WHEN sm.""ToWarehouseId"" = w.""Id"" THEN sm.""Quantity"" ELSE 0 END -
                                     CASE WHEN sm.""FromWarehouseId"" = w.""Id"" THEN sm.""Quantity"" ELSE 0 END
                             ), 0) AS Quantity
                FROM ""Warehouses"" as w
                         CROSS JOIN ""Products"" as p
                         LEFT JOIN ""StockMovements"" as sm
                                   ON sm.""ProductCode"" = p.""ProductCode""
                GROUP BY w.""Id"", w.""Name"", p.""ProductCode"", p.""Name""
                HAVING COALESCE(SUM(
                                        CASE WHEN sm.""ToWarehouseId"" = w.""Id"" THEN sm.""Quantity"" ELSE 0 END -
                                        CASE WHEN sm.""FromWarehouseId"" = w.""Id"" THEN sm.""Quantity"" ELSE 0 END
                                ), 0) <> 0
                ORDER BY w.""Id"", p.""ProductCode"";";

            var result = new List<WarehouseStock>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var warehouseId = reader.GetInt32(reader.GetOrdinal("WarehouseId"));
                var warehouseName = reader.IsDBNull("WarehouseName")
                    ? string.Empty
                    : reader.GetString("WarehouseName");

                var productCode = reader.GetString("ProductCode");
                var productName = reader.IsDBNull("ProductName")
                    ? string.Empty
                    : reader.GetString("ProductName");

                var quantity = reader.IsDBNull("Quantity")
                    ? 0
                    : reader.GetInt32("Quantity");

                result.Add(new WarehouseStock(
                    warehouseId,
                    warehouseName,
                    productCode,
                    productName,
                    quantity));
            }

            return result;
        }
    }
}