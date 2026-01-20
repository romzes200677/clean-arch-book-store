using MediatR;

namespace BookStore.User.Application.Commands.Verify;

public record VerifyTwoFactorCommand (Guid UserId, string Code):IRequest<string>;