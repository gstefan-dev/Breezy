using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Queries;

public class GetSessionByCodeQueryHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<GetSessionByCodeQueryHandler> _logger;

    public GetSessionByCodeQueryHandler(ISessionRepository sessionRepository,
        ILogger<GetSessionByCodeQueryHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<SessionResponse> Handle(GetSessionByCodeQuery query)
    {
        _logger.LogInformation("Getting session by code");
        var sessionCode = new SessionCode(query.SessionCode);

        var session = await _sessionRepository.GetByCodeAsync(sessionCode);
        if (session == null)
        {
            _logger.LogWarning("Session with {SessionCode} code not found",sessionCode);
            throw new DomainException("Session not found");
        }

        return new SessionResponse(session.Id, sessionCode.Value, session.Name, session.IsActive, session.OwnerUserId,
            session.CreatedAt);
    }
}