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
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var items = await _service.GetAll(cancellationToken);
        return Ok(items);
    }
}