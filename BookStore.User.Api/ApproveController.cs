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
public class ApproveController : ControllerBase
{
    private readonly IMediator _mediator; // Гейтвей в Application слой

    public ApproveController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery]ConfirmEmailDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(request.ToCommand(),cancellationToken);
        if (result.Success)
        {
            return Ok("email confirmed successfully");
        }
        return BadRequest($"email could not be confirmed ");
    }
   
    [HttpPost("2fa/verify-twofactor-token")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromBody] VerifyTwoFactorDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(request.ToCommand(),cancellationToken);
        return result switch
        {
            SuccessAuthResult s => Ok(new AuthResponseDto(false, s.AccessToken, s.RefreshToken)),
            FailedAuthResult f => BadRequest(f.ErrorMessage), // Четкий ответ пользователю
            _ => BadRequest("An unexpected error occurred")
        };
    }
}
