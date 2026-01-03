using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Api.commands;

namespace User.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator; // Гейтвей в Application слой

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
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
}