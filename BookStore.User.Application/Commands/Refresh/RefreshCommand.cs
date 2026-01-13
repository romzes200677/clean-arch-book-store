using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Commands.Refresh;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record RefreshCommand(
    string RefreshToken
) : IRequest<AuthenticationResult?>,ICommand;



