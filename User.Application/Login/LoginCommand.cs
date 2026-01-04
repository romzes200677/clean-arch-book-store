using MediatR;
using System.ComponentModel.DataAnnotations;

namespace User.Application.Login;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record LoginCommand(
    [Required] string Email, 
    [Required] string Password
) : IRequest<AuthenticationResult>;


