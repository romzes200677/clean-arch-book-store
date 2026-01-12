using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Refresh;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record RefreshCommand(
    string RefreshToken
) : IRequest<AuthenticationResult?>;



