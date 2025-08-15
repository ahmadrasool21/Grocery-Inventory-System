using GroceryInventory.Application.Abstractions;
using GroceryInventory.Application.DTOs;
using GroceryInventory.Domain.Entities;

namespace GroceryInventory.Application.Services;

public class StockService(IStockMovementRepository repo)
{
    public async Task<StockLevelDto> GetLevelAsync(Guid productId, CancellationToken ct = default)
        => new(productId, await repo.GetOnHandAsync(productId, ct));

    public Task<List<StockMovementDto>> GetMovementsAsync(Guid? productId, DateTime? from, DateTime? to, CancellationToken ct = default)
        => repo.GetMovementsAsync(productId, from, to, ct)
            .ContinueWith(t => t.Result
                .OrderByDescending(m => m.OccurredAt)
                .Select(m => new StockMovementDto(
                    m.Id, m.ProductId, m.Type.ToString(), m.Quantity,
                    m.Reason, m.BatchNo, m.ExpiryDate, m.OccurredAt)).ToList(), ct);

    public async Task RecordPurchaseAsync(MoveStockRequest req, CancellationToken ct = default)
        => await repo.AddAsync(new StockMovement
        {
            ProductId = req.ProductId,
            Type = StockMovementType.Purchase,
            Quantity = Math.Abs(req.Quantity),
            Reason = null,
            BatchNo = req.BatchNo,
            ExpiryDate = req.ExpiryDate,
            OccurredAt = DateTime.UtcNow
        }, ct);

    public async Task RecordSaleAsync(MoveStockRequest req, CancellationToken ct = default)
        => await repo.AddAsync(new StockMovement
        {
            ProductId = req.ProductId,
            Type = StockMovementType.Sale,
            Quantity = -Math.Abs(req.Quantity),
            Reason = req.Reason,
            BatchNo = req.BatchNo,
            ExpiryDate = req.ExpiryDate,
            OccurredAt = DateTime.UtcNow
        }, ct);

    public async Task RecordAdjustmentAsync(MoveStockRequest req, CancellationToken ct = default)
    {
        if (req.Quantity < 0 && string.IsNullOrWhiteSpace(req.Reason))
            throw new ArgumentException("Negative adjustments require a reason");

        await repo.AddAsync(new StockMovement
        {
            ProductId = req.ProductId,
            Type = StockMovementType.Adjust,
            Quantity = req.Quantity,
            Reason = req.Reason,
            BatchNo = req.BatchNo,
            ExpiryDate = req.ExpiryDate,
            OccurredAt = DateTime.UtcNow
        }, ct);
    }
}