namespace GroceryInventory.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty; // barcode/SKU
    public Guid CategoryId { get; set; }
    public string Unit { get; set; } = "pcs"; // e.g., pcs, kg, l
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal TaxRate { get; set; } // e.g., 0.10 for 10%
    public int ReorderLevel { get; set; } = 0;
    public bool IsBatchTracked { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}