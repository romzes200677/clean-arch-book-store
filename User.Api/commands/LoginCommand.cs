using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace User.Api.commands;

public record LoginCommand(
    [Required] string UserName, 
    [Required] string Password
) : IRequest<IResult>;