using BookStore.User.Application.Dto;
using MediatR;

namespace BookStore.User.Application.Commands.TwoFa.VerifyFA;

public record VerifyTwoFactorCommand (Guid UserId, string Token):IRequest<BaseAuthResult>;