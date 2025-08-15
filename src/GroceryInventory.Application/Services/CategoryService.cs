using GroceryInventory.Application.Abstractions;
using GroceryInventory.Application.DTOs;
using GroceryInventory.Domain.Entities;

namespace GroceryInventory.Application.Services;

public class CategoryService(ICategoryRepository repo)
{
    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        => (await repo.GetAllAsync(ct)).Select(c => new CategoryDto(c.Id, c.Name)).ToList();

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await repo.GetByIdAsync(id, ct)) is { } c ? new CategoryDto(c.Id, c.Name) : null;

    public async Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken ct = default)
    {
        var created = await repo.CreateAsync(new Category
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Name = dto.Name
        }, ct);
        return new CategoryDto(created.Id, created.Name);
    }

    public async Task UpdateAsync(Guid id, CategoryDto dto, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Category not found");
        existing.Name = dto.Name;
        await repo.UpdateAsync(existing, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => repo.DeleteAsync(id, ct);
}