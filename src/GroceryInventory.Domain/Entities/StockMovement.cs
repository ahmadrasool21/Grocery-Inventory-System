namespace GroceryInventory.Domain.Entities;

public enum StockMovementType { Purchase = 1, Sale = 2, Adjust = 3 }

public class StockMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; } // + purchase, - sale; adjust can be +/-
    public string? Reason { get; set; }   // required for negative adjustments
    public string? BatchNo { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}