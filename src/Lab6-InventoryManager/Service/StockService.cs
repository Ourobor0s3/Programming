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
                        // Проверяем, есть ли товар
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.ProductCode == productCode, ct);
                        if (product == null)
                            throw new InvalidOperationException($"Product '{productCode}' not found.");

                        // Считаем текущий остаток на исходном складе
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

        /// <summary>
        /// Создание склада
        /// </summary>
        public async Task<int> CreateWarehouseAsync(
            string name,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Warehouse name is required.", nameof(name));

            // Триггер стратегии выполнения
            return await _context.Database.CreateExecutionStrategy()
                .ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                    try
                    {
                        // Проверка уникальности имени склада (например, case-insensitive)
                        var existing = await _context.Warehouses
                            .Where(w => EF.Functions.ILike(w.Name, name)) // PostgreSQL-friendly
                            .FirstOrDefaultAsync(ct);

                        if (existing != null)
                            throw new InvalidOperationException($"Warehouse with name '{name}' already exists.");

                        var warehouse = new Warehouse
                        {
                            Name = name.Trim(),
                        };

                        _context.Warehouses.Add(warehouse);
                        await _context.SaveChangesAsync(ct);

                        await transaction.CommitAsync(ct);

                        return warehouse.Id; // возвращаем ID нового склада
                    }
                    catch
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                });
        }

        /// <summary>
        /// Пополнение склада из вне
        /// </summary>
        public async Task<bool> ReplenishWarehouseAsync(
            string productCode,
            int warehouseId,
            int quantity,
            CancellationToken ct = default)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.", nameof(quantity));

            if (warehouseId <= 0)
                throw new ArgumentException("Warehouse ID must be positive.", nameof(warehouseId));

            if (string.IsNullOrWhiteSpace(productCode))
                throw new ArgumentException("Product code is required.", nameof(productCode));

            return await _context.Database.CreateExecutionStrategy()
                .ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                    try
                    {
                        // Проверяем, существует ли склад
                        var warehouseExists = await _context.Warehouses
                            .AnyAsync(w => w.Id == warehouseId, ct);
                        if (!warehouseExists)
                            throw new InvalidOperationException($"Warehouse with ID {warehouseId} not found.");

                        // Проверяем, существует ли товар (опционально — зависит от требований)
                        var productExists = await _context.Products
                            .AnyAsync(p => p.ProductCode == productCode, ct);
                        if (!productExists)
                            throw new InvalidOperationException($"Product '{productCode}' not registered.");

                        // Записываем приход
                        var movement = new StockMovement
                        {
                            ProductCode = productCode,
                            ToWarehouseId = warehouseId,
                            Quantity = quantity,
                            When = DateTime.UtcNow,
                        };

                        _context.StockMovements.Add(movement);
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