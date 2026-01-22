using BookStore.User.Api.Dto;
using BookStore.User.Api.Extension;
using BookStore.User.Application.Commands.ChangePassword;
using BookStore.User.Application.Commands.ConfirmEmail;
using BookStore.User.Application.Commands.ForgotPassword.Prepare;
using BookStore.User.Application.Commands.ForgotPassword.ResetPassword;
using BookStore.User.Application.Commands.Login;
using BookStore.User.Application.Commands.RecoverConfirmEmail;
using BookStore.User.Application.Commands.Refresh;
using BookStore.User.Application.Commands.Register;
using BookStore.User.Application.Commands.TwoFa.Enable;
using BookStore.User.Application.Commands.TwoFa.VerifyFA;
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
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        // Вызываем MediatR
        var result = await _mediator.Send(command);

        return result switch
        {
            SuccessAuthResult success => Ok(success),
            RequiredTwoFactorResult mfa => Ok(new { 
                RequiresTwoFactor = true
            }),
            _ => BadRequest()
        };
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
        return BadRequest($"email could not be confirmed ");
    }
    
    [HttpPost("recovery/forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("recovery/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordCommand request)
    {
        var result=  await _mediator.Send(request);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword));
        return Ok();
    }
    
    [Authorize]
    [HttpGet("get-profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var roles = await _mediator.Send(new GetRolesQuery(userId));
        return Ok(roles);
    }
    
    [HttpPost("2fa/verify-twofactor-token")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromBody] VerifyTwoFactorCommand request)
    {
        var result = await _mediator.Send(request);

        return result switch
        {
            SuccessAuthResult success => Ok(success),
            RequiredTwoFactorResult mfa => Ok(new { 
                RequiresTwoFactor = true
            }),
            _ => BadRequest()
        };
    }
    
    [HttpPost("2fa/enable-twofactor")]
    public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactor request)
    {
        await _mediator.Send(request);
        return NoContent();
    }
    
    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmail request)
    {
        await _mediator.Send(request);
        return NoContent();
    }
}
