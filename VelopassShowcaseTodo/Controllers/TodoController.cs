using Domain;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace VelopassShowcaseTodo.Controllers;

[ApiController]
[Route("/todos")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAll(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.Get(id, cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    public record TodoCreateRequest(string Name, bool Done);
    public record TodoUpdateRequest(string Name, bool Done);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoCreateRequest request, CancellationToken cancellationToken)
    {
        if (request is null) return BadRequest();
        var created = await _service.Create(request.Name, request.Done, cancellationToken);
        return CreatedAtRoute(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TodoUpdateRequest request, CancellationToken cancellationToken)
    {
        if (request is null) return BadRequest();
        var success = await _service.Update(id, request.Name, request.Done, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var success = await _service.Delete(id, cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}