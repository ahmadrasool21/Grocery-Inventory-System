// using GroceryInventory.Application.DTOs;
// using GroceryInventory.Application.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace GroceryInventory.Api.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class ProductsController(IProductService productService) : ControllerBase
// {
//     [HttpGet]
//     public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken ct)
//         => Ok(await productService.GetAllAsync(ct));

//     [HttpGet("{id:guid}")]
//     public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
//     {
//         var p = await productService.GetByIdAsync(id, ct);
//         return p is null ? NotFound() : Ok(p);
//     }

//     [HttpPost]
//     public async Task<ActionResult<ProductDto>> Create([FromBody] ProductDto dto, CancellationToken ct)
//     {
//         var created = await productService.CreateAsync(dto with { Id = Guid.Empty, CreatedAt = DateTime.UtcNow }, ct);
//         return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
//     }

//     [HttpPut("{id:guid}")]
//     public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto, CancellationToken ct)
//     {
//         await productService.UpdateAsync(id, dto, ct);
//         return NoContent();
//     }

//     [HttpDelete("{id:guid}")]
//     public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//     {
//         await productService.DeleteAsync(id, ct);
//         return NoContent();
//     }
// }








using GroceryInventory.Application.DTOs;
using GroceryInventory.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // ← add this

namespace GroceryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ClerkOrAdmin")] // ← class-level: requires at least Clerk for all actions
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken ct)
        => Ok(await productService.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
    {
        var p = await productService.GetByIdAsync(id, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")] // ← only Admin can create
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductDto dto, CancellationToken ct)
    {
        var created = await productService.CreateAsync(dto with { Id = Guid.Empty, CreatedAt = DateTime.UtcNow }, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")] // ← only Admin can update
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto, CancellationToken ct)
    {
        await productService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")] // ← only Admin can delete
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await productService.DeleteAsync(id, ct);
        return NoContent();
    }
}
