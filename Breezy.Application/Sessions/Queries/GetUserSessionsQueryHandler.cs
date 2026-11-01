using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using MediatR;

namespace Breezy.Application.Sessions.Queries;

public class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, IEnumerable<UserSessionResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetUserSessionsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IEnumerable<UserSessionResponse>> Handle(GetUserSessionsQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetByParticipantAsync(query.UserId, query.IncludeArchived);

        return sessions.Select(s =>
        {
            var me = s.Participants.First(p => p.UserId == query.UserId);

            var myStatus = me.IsActive
                ? "Member"
                : me.ExitReason == ParticipantExitReason.RemovedByOwner ? "RemovedByOwner" : "Left";

            return new UserSessionResponse(s.Id, s.Code.Value, s.Name, s.IsActive, s.OwnerUserId,
                s.CreatedAt, s.Visibility, myStatus, me.IsArchived);
        });
    }
}