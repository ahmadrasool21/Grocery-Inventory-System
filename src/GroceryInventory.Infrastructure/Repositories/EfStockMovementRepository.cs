using GroceryInventory.Application.Abstractions;
using GroceryInventory.Domain.Entities;
using GroceryInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GroceryInventory.Infrastructure.Repositories;

public class EfStockMovementRepository(AppDbContext db) : IStockMovementRepository
{
    public async Task AddAsync(StockMovement movement, CancellationToken ct = default)
    {
        db.StockMovements.Add(movement);
        await db.SaveChangesAsync(ct);
    }

    // public async Task<decimal> GetOnHandAsync(Guid productId, CancellationToken ct = default)
    // {
    //     var sum = await db.StockMovements
    //         .Where(m => m.ProductId == productId)
    //         .SumAsync(m => (decimal?)m.Quantity, ct) ?? 0m;
    //     return sum;
    // }

    public async Task<decimal> GetOnHandAsync(Guid productId, CancellationToken ct = default)
{
    var sum = await db.StockMovements
        .Where(m => m.ProductId == productId)
        .Select(m => (double)m.Quantity) // cast to double for SQLite
        .SumAsync(ct);

    return (decimal)sum; // convert back to decimal
}

    public async Task<List<StockMovement>> GetMovementsAsync(Guid? productId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = db.StockMovements.AsNoTracking().AsQueryable();
        if (productId is Guid pid) q = q.Where(m => m.ProductId == pid);
        if (from is DateTime f)   q = q.Where(m => m.OccurredAt >= f);
        if (to   is DateTime t)   q = q.Where(m => m.OccurredAt <= t);
        return await q.OrderByDescending(m => m.OccurredAt).ToListAsync(ct);
    }
}