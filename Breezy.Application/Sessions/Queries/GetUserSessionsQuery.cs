using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Queries;

public record GetUserSessionsQuery(Guid UserId, bool IncludeArchived) : IRequest<IEnumerable<UserSessionResponse>>;