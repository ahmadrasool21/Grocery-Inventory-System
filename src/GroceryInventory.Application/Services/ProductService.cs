using GroceryInventory.Application.Abstractions;
using GroceryInventory.Application.DTOs;
using GroceryInventory.Domain.Entities;

namespace GroceryInventory.Application.Services;

public class ProductService(IProductRepository repo) : IProductService
{
    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Sku, p.CategoryId, p.Unit, p.CostPrice, p.SalePrice,
        p.TaxRate, p.ReorderLevel, p.IsBatchTracked, p.IsActive, p.CreatedAt);

    private static Product FromDto(ProductDto d) => new()
    {
        Id = d.Id == Guid.Empty ? Guid.NewGuid() : d.Id,
        Name = d.Name,
        Sku = d.Sku,
        CategoryId = d.CategoryId,
        Unit = d.Unit,
        CostPrice = d.CostPrice,
        SalePrice = d.SalePrice,
        TaxRate = d.TaxRate,
        ReorderLevel = d.ReorderLevel,
        IsBatchTracked = d.IsBatchTracked,
        IsActive = d.IsActive,
        CreatedAt = d.CreatedAt == default ? DateTime.UtcNow : d.CreatedAt
    };

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
        => (await repo.GetAllAsync(ct)).Select(ToDto).ToList();

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await repo.GetByIdAsync(id, ct)) is { } p ? ToDto(p) : null;

    public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken ct = default)
    {
        var entity = FromDto(dto);
        var created = await repo.CreateAsync(entity, ct);
        return ToDto(created);
    }

    public async Task UpdateAsync(Guid id, ProductDto dto, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Product not found");
        existing.Name = dto.Name;
        existing.Sku = dto.Sku;
        existing.CategoryId = dto.CategoryId;
        existing.Unit = dto.Unit;
        existing.CostPrice = dto.CostPrice;
        existing.SalePrice = dto.SalePrice;
        existing.TaxRate = dto.TaxRate;
        existing.ReorderLevel = dto.ReorderLevel;
        existing.IsBatchTracked = dto.IsBatchTracked;
        existing.IsActive = dto.IsActive;
        await repo.UpdateAsync(existing, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => repo.DeleteAsync(id, ct);
}