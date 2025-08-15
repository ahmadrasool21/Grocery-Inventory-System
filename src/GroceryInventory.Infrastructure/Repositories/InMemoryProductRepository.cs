using System.Collections.Concurrent;
using GroceryInventory.Application.Abstractions;
using GroceryInventory.Domain.Entities;

namespace GroceryInventory.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _store = new();

    public InMemoryProductRepository()
    {
        // Seed sample products
        var catId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var p1 = new Product { Name = "Banana", Sku = "SKU-BAN-001", CategoryId = catId, Unit = "kg", CostPrice = 1.00m, SalePrice = 1.50m, TaxRate = 0.10m, ReorderLevel = 10, IsBatchTracked = false, IsActive = true, CreatedAt = now };
        var p2 = new Product { Name = "Milk 1L", Sku = "SKU-MLK-001", CategoryId = catId, Unit = "pcs", CostPrice = 0.70m, SalePrice = 1.20m, TaxRate = 0.10m, ReorderLevel = 15, IsBatchTracked = true, IsActive = true, CreatedAt = now };
        _store[p1.Id] = p1;
        _store[p2.Id] = p2;
    }

    public Task<List<Product>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult(_store.Values.OrderBy(p => p.Name).ToList());

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var p);
        return Task.FromResult(p);
    }

    public Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        _store[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _store[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}