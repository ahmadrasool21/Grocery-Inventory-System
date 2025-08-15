// using GroceryInventory.Application.DTOs;
// using GroceryInventory.Application.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace GroceryInventory.Api.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class CategoriesController(CategoryService service) : ControllerBase
// {
//     [HttpGet]
//     public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken ct)
//         => Ok(await service.GetAllAsync(ct));

//     [HttpGet("{id:guid}")]
//     public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct)
//     {
//         var c = await service.GetByIdAsync(id, ct);
//         return c is null ? NotFound() : Ok(c);
//     }

//     [HttpPost]
//     public async Task<ActionResult<CategoryDto>> Create(CategoryDto dto, CancellationToken ct)
//     {
//         var created = await service.CreateAsync(dto with { Id = Guid.Empty }, ct);
//         return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
//     }

//     [HttpPut("{id:guid}")]
//     public async Task<IActionResult> Update(Guid id, CategoryDto dto, CancellationToken ct)
//     {
//         await service.UpdateAsync(id, dto, ct);
//         return NoContent();
//     }

//     [HttpDelete("{id:guid}")]
//     public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//     {
//         await service.DeleteAsync(id, ct);
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
[Authorize(Policy = "ClerkOrAdmin")] // ← class-level: needs Clerk or Admin for all actions
public class CategoriesController(CategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct)
    {
        var c = await service.GetByIdAsync(id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")] // ← only Admin can create
    public async Task<ActionResult<CategoryDto>> Create(CategoryDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto with { Id = Guid.Empty }, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")] // ← only Admin can update
    public async Task<IActionResult> Update(Guid id, CategoryDto dto, CancellationToken ct)
    {
        await service.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")] // ← only Admin can delete
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
