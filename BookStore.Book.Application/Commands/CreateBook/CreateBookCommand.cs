using MediatR;

namespace BookStore.Book.Application.Commands.CreateBook;

public record CreateBookCommand(string Email, string Password) : IRequest<CreateBookResponse>;