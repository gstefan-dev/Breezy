using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Queries;

public class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, IEnumerable<SessionResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetUserSessionsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IEnumerable<SessionResponse>> Handle(GetUserSessionsQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetByParticipantAsync(query.UserId);

        return sessions.Select(s => new SessionResponse(
            s.Id, s.Code.Value, s.Name, s.IsActive, s.OwnerUserId, s.CreatedAt, s.Visibility));
    }
}