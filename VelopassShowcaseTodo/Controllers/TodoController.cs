using Microsoft.AspNetCore.Mvc;

namespace VelopassShowcaseTodo.Controllers;

[ApiController]
[Route("/todos")]
public class TodoController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Hello World!";
    }
}