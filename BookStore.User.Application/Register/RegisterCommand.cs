using System.ComponentModel.DataAnnotations;
using MediatR;

namespace BookStore.User.Application.Register;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record RegisterCommand(
    [Required] string Email, 
    [Required] string Password
) : IRequest;


