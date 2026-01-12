using System.ComponentModel.DataAnnotations;
using BookStore.User.Application.Interfaces.Features;
using MediatR;

namespace BookStore.User.Application.Login;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record LoginCommand(
    [Required] string Email, 
    [Required] string Password
) : IRequest<AuthenticationResult>;


