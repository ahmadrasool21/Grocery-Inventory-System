using FluentAssertions;
using GroceryInventory.Application.Abstractions;
using GroceryInventory.Application.DTOs;
using GroceryInventory.Application.Services;
using GroceryInventory.Domain.Entities;

namespace GroceryInventory.UnitTests.Services;

public class StockServiceTests
{
    private class FakeStockRepo : IStockMovementRepository
    {
        public readonly List<StockMovement> Store = new();

        public Task AddAsync(StockMovement movement, CancellationToken ct = default)
        {
            Store.Add(movement);
            return Task.CompletedTask;
        }
        public Task<decimal> GetOnHandAsync(Guid productId, CancellationToken ct = default)
            => Task.FromResult(Store.Where(x => x.ProductId == productId).Sum(x => x.Quantity));
        public Task<List<StockMovement>> GetMovementsAsync(Guid? productId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            IEnumerable<StockMovement> q = Store;
            if (productId is Guid pid) q = q.Where(m => m.ProductId == pid);
            if (from is DateTime f) q = q.Where(m => m.OccurredAt >= f);
            if (to is DateTime t) q = q.Where(m => m.OccurredAt <= t);
            return Task.FromResult(q.OrderByDescending(m => m.OccurredAt).ToList());
        }
    }

    [Fact]
    public async Task Purchase_increases_on_hand_and_sale_decreases()
    {
        var repo = new FakeStockRepo();
        var service = new StockService(repo);
        var pid = Guid.NewGuid();

        await service.RecordPurchaseAsync(new MoveStockRequest(pid, 10, 1m, null, null, null));
        (await service.GetLevelAsync(pid)).OnHand.Should().Be(10);

        await service.RecordSaleAsync(new MoveStockRequest(pid, 3, null, "sale", null, null));
        (await service.GetLevelAsync(pid)).OnHand.Should().Be(7);
    }

    [Fact]
    public async Task Negative_adjust_requires_reason()
    {
        var repo = new FakeStockRepo();
        var service = new StockService(repo);
        var pid = Guid.NewGuid();

        var act = () => service.RecordAdjustmentAsync(new MoveStockRequest(pid, -2, null, null, null, null));
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*reason*");
    }
}