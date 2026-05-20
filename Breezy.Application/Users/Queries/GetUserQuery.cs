using Breezy.Application.Users.DTOs;
using MediatR;

namespace Breezy.Application.Users.Queries;

public record GetUserQuery(Guid UserId) : IRequest<UserResponse>;
