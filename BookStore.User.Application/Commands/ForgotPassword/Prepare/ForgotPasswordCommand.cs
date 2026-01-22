using MediatR;

namespace BookStore.User.Application.Commands.ForgotPassword.Prepare;

public class ForgotPasswordCommand  : IRequest
{
    public string  Email { get; set; }
}