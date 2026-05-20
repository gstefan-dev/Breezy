using Breezy.Application.Users.DTOs;
using MediatR;

namespace Breezy.Application.Users.Commands;

public record RegisterUserCommand(string UserName, string UserEmail, string Password) : IRequest<UserResponse>;