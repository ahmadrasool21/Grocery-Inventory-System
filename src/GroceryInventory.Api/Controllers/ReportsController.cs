using GroceryInventory.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GroceryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IProductRepository products, IStockMovementRepository stockRepo) : ControllerBase
{
    [HttpGet("stock-summary")]
    public async Task<IActionResult> StockSummary(CancellationToken ct)
    {
        var list = await products.GetAllAsync(ct);
        var rows = new List<object>();
        foreach (var p in list)
        {
            var onHand = await stockRepo.GetOnHandAsync(p.Id, ct);
            rows.Add(new { p.Id, p.Name, p.Sku, OnHand = onHand, ReorderLevel = p.ReorderLevel, Low = onHand < p.ReorderLevel });
        }
        var ordered = rows.OrderBy(r => (string)r.GetType().GetProperty("Name")!.GetValue(r)!);
        return Ok(ordered);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock(CancellationToken ct)
    {
        var list = await products.GetAllAsync(ct);
        var result = new List<object>();
        foreach (var p in list)
        {
            var onHand = await stockRepo.GetOnHandAsync(p.Id, ct);
            if (onHand < p.ReorderLevel)
                result.Add(new { p.Id, p.Name, p.Sku, OnHand = onHand, p.ReorderLevel });
        }
        var ordered = result.OrderBy(r => (string)r.GetType().GetProperty("Name")!.GetValue(r)!);
        return Ok(ordered);
    }
}