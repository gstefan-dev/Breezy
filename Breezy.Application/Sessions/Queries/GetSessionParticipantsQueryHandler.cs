using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;

namespace Breezy.Application.Sessions.Queries;

public class GetSessionParticipantsQueryHandler : IRequestHandler<GetSessionParticipantsQuery, IEnumerable<ParticipantResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionParticipantsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IEnumerable<ParticipantResponse>> Handle(GetSessionParticipantsQuery request,
        CancellationToken cancellationToken)
    {
        var code = new SessionCode(request.Code);
        var session = await _sessionRepository.GetByCodeAsync(code);

        if (session == null)
            throw new DomainException("Session not found");

        return session.Participants.Select(p =>
            new ParticipantResponse(p.Id, p.SessionId, p.UserId, p.UserName, p.JoinedAt,
                p.LeftAt, p.ExitReason?.ToString()));
    }
}