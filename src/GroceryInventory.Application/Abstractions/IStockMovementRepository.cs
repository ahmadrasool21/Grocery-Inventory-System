using GroceryInventory.Domain.Entities;

namespace GroceryInventory.Application.Abstractions;

public interface IStockMovementRepository
{
    Task AddAsync(StockMovement movement, CancellationToken ct = default);
    Task<decimal> GetOnHandAsync(Guid productId, CancellationToken ct = default);
    Task<List<StockMovement>> GetMovementsAsync(Guid? productId, DateTime? from, DateTime? to, CancellationToken ct = default);
}