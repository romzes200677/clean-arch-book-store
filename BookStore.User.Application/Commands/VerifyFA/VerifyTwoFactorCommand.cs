using BookStore.User.Api.Dto;
using BookStore.User.Application.Interfaces;
using MediatR;

namespace BookStore.User.Application.Commands.VerifyFA;

public record VerifyTwoFactorCommand (Guid UserId, string Token):IRequest<BaseAuthResult>;