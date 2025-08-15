using GroceryInventory.Application.Abstractions;
using GroceryInventory.Domain.Entities;
using GroceryInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GroceryInventory.Infrastructure.Repositories;

public class EfProductRepository(AppDbContext db) : IProductRepository
{
    public Task<List<Product>> GetAllAsync(CancellationToken ct = default)
        => db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        Console.WriteLine(product); 
        db.Products.Update(product);

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity is null) return;
        db.Products.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}