using GroceryInventory.Application.DTOs;

namespace GroceryInventory.Application.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, ProductDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}