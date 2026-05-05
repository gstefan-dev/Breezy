using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class ReopenSessionCommandHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<ReopenSessionCommandHandler> _logger;

    public ReopenSessionCommandHandler(ISessionRepository sessionRepository, ILogger<ReopenSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<SessionResponse> Handle(ReopenSessionCommand command)
    {
        var sessionCode = new SessionCode(command.SessionCode);

        var session = await _sessionRepository.GetByCodeAsync(sessionCode);

        if (session == null)
        {
            _logger.LogWarning("Session not found for SessionCode: {SessionCode} ", sessionCode);
            throw new DomainException("Session not found");
        }
        
        session.Reopen(command.UserId);

        await _sessionRepository.UpdateAsync(session);
        
        _logger.LogInformation("Session is reopened successfully by OwnerUser: {UserId}", command.UserId );

        return new SessionResponse(session.Id, sessionCode.Value, session.Name, session.IsActive, session.OwnerUserId,
            session.CreatedAt);
    }
}