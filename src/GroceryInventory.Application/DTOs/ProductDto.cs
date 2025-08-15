namespace GroceryInventory.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    Guid CategoryId,
    string Unit,
    decimal CostPrice,
    decimal SalePrice,
    decimal TaxRate,
    int ReorderLevel,
    bool IsBatchTracked,
    bool IsActive,
    DateTime CreatedAt
);