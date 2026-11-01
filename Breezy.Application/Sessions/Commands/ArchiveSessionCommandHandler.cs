using Breezy.Application.Interfaces;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class ArchiveSessionCommandHandler : IRequestHandler<ArchiveSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<ArchiveSessionCommandHandler> _logger;

    public ArchiveSessionCommandHandler(ISessionRepository sessionRepository, ILogger<ArchiveSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task Handle(ArchiveSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByCodeAsync(new SessionCode(request.SessionCode))
                      ?? throw new DomainException("Session not found");

        session.ArchiveForUser(request.UserId);
        await _sessionRepository.UpdateAsync(session);

        _logger.LogInformation("User {UserId} archived session {SessionCode}", request.UserId, request.SessionCode);
    }
}