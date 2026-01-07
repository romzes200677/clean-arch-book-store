using System.Security.Claims;
using BookStore.User.Application.ConfirmEmail;
using BookStore.User.Application.Login;
using BookStore.User.Application.Refresh;
using BookStore.User.Application.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

// Новый импорт

namespace BookStore.User.Api;

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
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        // Если используете MediatR:
        await _mediator.Send(new RegisterCommand(request.Email, request.Password));
         
        return Ok("User registered successfully");
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand request)
    {
        var result =await _mediator.Send(request);
    
        if (result == null)
            return Unauthorized("Невалидный токен обновления");

        return Ok(result);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(Guid userId,string token)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand(userId,token));
        if (result.Success)
        {
            return Ok("email confirmed successfully");
        }
        return BadRequest($"email could not be confirmed with errors: {result.Errors} ");
    }
}
