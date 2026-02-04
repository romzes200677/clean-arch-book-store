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
public class RestoreController : ControllerBase
{
    private readonly IMediator _mediator; // Гейтвей в Application слой

    public RestoreController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost("recovery/forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request.ToCommand(),cancellationToken);
        return Ok();
    }
    
    [HttpPost("recovery/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDto request,
        CancellationToken cancellationToken = default)
    {
        var result=  await _mediator.Send(request.ToCommand(),cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailDto request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request.ToCommand(),cancellationToken);
        return NoContent();
    }
}
