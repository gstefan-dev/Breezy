using Breezy.Application.Interfaces;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class RestoreSessionCommandHandler : IRequestHandler<RestoreSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<RestoreSessionCommandHandler> _logger;

    public RestoreSessionCommandHandler(ISessionRepository sessionRepository, ILogger<RestoreSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task Handle(RestoreSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByCodeAsync(new SessionCode(request.SessionCode))
                      ?? throw new DomainException("Session not found");

        session.RestoreForUser(request.UserId);
        await _sessionRepository.UpdateAsync(session);

        _logger.LogInformation("User {UserId} restored session {SessionCode}", request.UserId, request.SessionCode);
    }
}