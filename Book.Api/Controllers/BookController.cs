using System.Security.Claims;
using Book.Application.Commands.CreateBook;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IMediator _mediator; // Гейтвей в Application слой

    public BookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CreateBookCommand command)
    {
        // Вызываем MediatR (или хендлер напрямую)
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize] // Токен валиден?
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        // Достаем данные из токена (Claims)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(new { UserId = userId });
    }
}
