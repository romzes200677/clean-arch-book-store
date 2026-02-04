using BookStore.User.Api.Dto;
using BookStore.User.Api.Extension;
using BookStore.User.Application.Dto;
using BookStore.User.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Login([FromBody] LoginDto command,
        CancellationToken cancellationToken = default)
    {
        // Вызываем MediatR
        var result = await _mediator.Send(command.ToCommand());

        return result switch
        {
            SuccessAuthResult s => Ok(new AuthResponseDto(false, s.AccessToken, s.RefreshToken)),
            FailedAuthResult f => BadRequest(f.ErrorMessage),
            _ => BadRequest()
        };
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request,
        CancellationToken cancellationToken = default)
    {
        // Если используете MediatR:
        await _mediator.Send(request.ToCommand(),cancellationToken);
         
        return Ok("User registered successfully");
    }
    
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto request,
        CancellationToken cancellationToken = default)
    {
        var result =await _mediator.Send(request.ToCommand(),cancellationToken);
    
        if (result == null)
            return Unauthorized("Невалидный токен обновления");

        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        await _mediator.Send(request.ToCommand(userId),cancellationToken);
        return Ok();
    }
    
    [Authorize]
    [HttpGet("get-profile")]
    public async Task<IActionResult> GetProfile(
    CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        var roles = await _mediator.Send(new GetRolesQuery(userId),cancellationToken);
        return Ok(roles);
    }
    
    
    [Authorize]
    [HttpPost("2fa/enable-twofactor")]
    public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorDto request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request.ToCommand(),cancellationToken);
        return NoContent();
    }
    
    
}
