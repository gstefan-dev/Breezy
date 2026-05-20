using Breezy.Application.Users.DTOs;
using MediatR;

namespace Breezy.Application.Users.Commands;

public record LoginCommand(string Email, string Password) : IRequest<string>;

