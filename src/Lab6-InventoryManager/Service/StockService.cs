using Lab6_InventoryManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab6_InventoryManager.Service
{
    public class StockService
    {
        private readonly DataContext _context;

        public StockService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Переместить товар между складами с проверкой остатка на исходном складе.
        /// </summary>
        public async Task<bool> MoveStockAsync(
            string productCode,
            int fromWarehouseId,
            int toWarehouseId,
            int quantity,
            CancellationToken ct = default)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(quantity));

            if (fromWarehouseId == toWarehouseId)
                throw new ArgumentException("From and To warehouses must be different.");

            return await _context.Database.CreateExecutionStrategy()
                .ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                    try
                    {
                        // 🔍 Проверяем, есть ли товар
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.ProductCode == productCode, ct);
                        if (product == null)
                            throw new InvalidOperationException($"Product '{productCode}' not found.");

                        // 🔍 Считаем текущий остаток на исходном складе
                        var currentStockAtFrom = await _context.StockMovements
                            .Where(sm => sm.ProductCode == productCode)
                            .GroupBy(sm => 1) // dummy group
                            .Select(g => g.Sum(sm =>
                                (sm.ToWarehouseId == fromWarehouseId ? sm.Quantity : 0) -
                                (sm.FromWarehouseId == fromWarehouseId ? sm.Quantity : 0)))
                            .FirstOrDefaultAsync(ct);

                        if (currentStockAtFrom < quantity)
                        {
                            throw new InvalidOperationException(
                                $"Insufficient stock at warehouse {fromWarehouseId}. Available: {currentStockAtFrom}, requested: {quantity}.");
                        }

                        // Записываем перемещение
                        var movement = new StockMovement
                        {
                            ProductCode = productCode,
                            FromWarehouseId = fromWarehouseId,
                            ToWarehouseId = toWarehouseId,
                            Quantity = quantity,
                            When = DateTime.UtcNow
                        };

                        _context.StockMovements.Add(movement);

                        // Обновляем общий остаток продукта (опционально — можно не хранить, а считать)
                        // product.Stock не меняется при внутреннем перемещении!
                        // (меняется только при приходе/списании)
                        await _context.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);
                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                });
        }
    }
}