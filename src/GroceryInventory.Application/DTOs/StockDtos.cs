namespace GroceryInventory.Application.DTOs;

public record StockMovementDto(
    Guid Id, Guid ProductId, string Type, decimal Quantity,
    string? Reason, string? BatchNo, DateTime? ExpiryDate, DateTime OccurredAt);

public record MoveStockRequest(
    Guid ProductId, decimal Quantity, decimal? UnitCost,
    string? Reason, string? BatchNo, DateTime? ExpiryDate);

public record StockLevelDto(Guid ProductId, decimal OnHand);