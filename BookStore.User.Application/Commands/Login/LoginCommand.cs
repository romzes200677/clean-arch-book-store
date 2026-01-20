using System.ComponentModel.DataAnnotations;
using BookStore.User.Application.Interfaces;
using BookStore.User.Application.Interfaces.Utils;
using MediatR;

namespace BookStore.User.Application.Commands.Login;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record LoginCommand(
    [Required] string Email, 
    [Required] string Password
) : IRequest<AuthenticationResult>,ICommand;


