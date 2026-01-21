using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword;

public class ForgotPasswordCommand  : IRequest
{
    public string  Email { get; set; }
}