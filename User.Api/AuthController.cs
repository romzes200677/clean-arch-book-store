using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.Login; // Новый импорт

namespace User.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator; // Гейтвей в Application слой

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        // Вызываем MediatR
        var result = await _mediator.Send(command);
        
        // result это AuthenticationResult { Token, UserId }
        return Ok(new { Token = result.Token, UserId = result.UserId });
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
