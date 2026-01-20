using BookStore.User.Application.Interfaces.Utils;
using MediatR;

namespace BookStore.User.Application.Commands.Register;

// Команда должна ссылаться на результат, который мы ожидаем после работы
public record RegisterCommand(
    string Email, 
    string Password
) : IRequest,ICommand;


