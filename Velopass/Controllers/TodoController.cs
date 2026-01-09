using Domain;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace VelopassShowcaseTodo.Controllers;

[ApiController]
[Route("/todos")]
public class TodoController(ITodoService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await service.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await service.Get(id, cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    public record TodoCreateRequest(string Name);
    public record TodoUpdateRequest(string? Name, bool? Done);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await service.CreateAsync(request.Name, cancellationToken);
        return CreatedAtRoute(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TodoUpdateRequest request, CancellationToken cancellationToken)
    {
        var success = await service.UpdateAsync(id, request.Name, request.Done, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var success = await service.DeleteAsync(id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}