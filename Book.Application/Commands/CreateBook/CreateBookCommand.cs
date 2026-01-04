using MediatR;

namespace Book.Application.Commands.CreateBook;

public record CreateBookCommand(string Email, string Password) : IRequest<CreateBookResponse>;