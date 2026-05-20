using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class ReopenSessionCommandHandler : IRequestHandler<ReopenSessionCommand, SessionResponse>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<ReopenSessionCommandHandler> _logger;

    public ReopenSessionCommandHandler(ISessionRepository sessionRepository, ILogger<ReopenSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<SessionResponse> Handle(ReopenSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionCode = new SessionCode(request.SessionCode);

        var session = await _sessionRepository.GetByCodeAsync(sessionCode);

        if (session == null)
        {
            _logger.LogWarning("Session not found for SessionCode: {SessionCode} ", sessionCode);
            throw new DomainException("Session not found");
        }
        
        session.Reopen(request.UserId);

        await _sessionRepository.UpdateAsync(session);
        
        _logger.LogInformation("Session is reopened successfully by OwnerUser: {UserId}", request.UserId );

        return new SessionResponse(session.Id, sessionCode.Value, session.Name, session.IsActive, session.OwnerUserId,
            session.CreatedAt);
    }
}