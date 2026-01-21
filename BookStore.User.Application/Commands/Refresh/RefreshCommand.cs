using BookStore.User.Api.Dto;
using BookStore.User.Application.Interfaces.Utils;
using MediatR;

namespace BookStore.User.Application.Commands.Refresh;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record RefreshCommand(
    string RefreshToken
) : IRequest<SuccessAuthResult?>,ICommand;



