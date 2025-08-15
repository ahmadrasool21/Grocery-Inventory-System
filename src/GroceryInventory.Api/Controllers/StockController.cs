// using GroceryInventory.Application.DTOs;
// using GroceryInventory.Application.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace GroceryInventory.Api.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class StockController(StockService stock) : ControllerBase
// {
//     [HttpGet("level/{productId:guid}")]
//     public async Task<ActionResult<StockLevelDto>> GetLevel(Guid productId, CancellationToken ct)
//         => Ok(await stock.GetLevelAsync(productId, ct));

//     [HttpGet("movements")]
//     public async Task<ActionResult<List<StockMovementDto>>> GetMovements(
//         [FromQuery] Guid? productId,
//         [FromQuery] DateTime? from,
//         [FromQuery] DateTime? to,
//         CancellationToken ct)
//         => Ok(await stock.GetMovementsAsync(productId, from, to, ct));

//     [HttpPost("purchase")]
//     public async Task<IActionResult> Purchase([FromBody] MoveStockRequest req, CancellationToken ct)
//     {
//         if (req.Quantity <= 0) return BadRequest("Quantity must be positive");
//         await stock.RecordPurchaseAsync(req, ct);
//         return Accepted();
//     }

//     [HttpPost("sale")]
//     public async Task<IActionResult> Sale([FromBody] MoveStockRequest req, CancellationToken ct)
//     {
//         if (req.Quantity <= 0) return BadRequest("Quantity must be positive");
//         await stock.RecordSaleAsync(req, ct);
//         return Accepted();
//     }

//     [HttpPost("adjust")]
//     public async Task<IActionResult> Adjust([FromBody] MoveStockRequest req, CancellationToken ct)
//     {
//         if (req.Quantity == 0) return BadRequest("Quantity cannot be zero");
//         await stock.RecordAdjustmentAsync(req, ct);
//         return Accepted();
//     }
// }



using GroceryInventory.Application.DTOs;
using GroceryInventory.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // ← add this

namespace GroceryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ClerkOrAdmin")] // ← require Clerk or Admin for all actions
public class StockController(StockService stock) : ControllerBase
{
    [HttpGet("level/{productId:guid}")]
    public async Task<ActionResult<StockLevelDto>> GetLevel(Guid productId, CancellationToken ct)
        => Ok(await stock.GetLevelAsync(productId, ct));

    [HttpGet("movements")]
    public async Task<ActionResult<List<StockMovementDto>>> GetMovements(
        [FromQuery] Guid? productId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
        => Ok(await stock.GetMovementsAsync(productId, from, to, ct));

    [HttpPost("purchase")]
    [Authorize(Policy = "Admin")] // ← only Admin can receive stock
    public async Task<IActionResult> Purchase([FromBody] MoveStockRequest req, CancellationToken ct)
    {
        if (req.Quantity <= 0)
        {
            ModelState.AddModelError(nameof(req.Quantity), "Quantity must be positive");
            return ValidationProblem(ModelState); // consistent ProblemDetails (400)
        }
        await stock.RecordPurchaseAsync(req, ct);
        return Accepted();
    }

    [HttpPost("sale")]
    [Authorize(Policy = "ClerkOrAdmin")] // ← Clerk or Admin can sell
    public async Task<IActionResult> Sale([FromBody] MoveStockRequest req, CancellationToken ct)
    {
        if (req.Quantity <= 0)
        {
            ModelState.AddModelError(nameof(req.Quantity), "Quantity must be positive");
            return ValidationProblem(ModelState);
        }
        await stock.RecordSaleAsync(req, ct);
        return Accepted();
    }

    [HttpPost("adjust")]
    [Authorize(Policy = "Admin")] // ← only Admin can adjust
    public async Task<IActionResult> Adjust([FromBody] MoveStockRequest req, CancellationToken ct)
    {
        if (req.Quantity == 0)
        {
            ModelState.AddModelError(nameof(req.Quantity), "Quantity cannot be zero");
            return ValidationProblem(ModelState);
        }
        if (req.Quantity < 0 && string.IsNullOrWhiteSpace(req.Reason))
        {
            ModelState.AddModelError(nameof(req.Reason), "Reason is required for negative movements");
            return ValidationProblem(ModelState);
        }
        await stock.RecordAdjustmentAsync(req, ct);
        return Accepted();
    }
}
