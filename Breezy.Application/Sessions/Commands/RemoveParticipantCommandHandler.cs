using Breezy.Application.Interfaces;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class RemoveParticipantCommandHandler : IRequestHandler<RemoveParticipantCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<RemoveParticipantCommandHandler> _logger;

    public RemoveParticipantCommandHandler(ISessionRepository sessionRepository, ILogger<RemoveParticipantCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task Handle(RemoveParticipantCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByCodeAsync(new SessionCode(request.SessionCode))
                      ?? throw new DomainException("Session not found");

        session.RemoveParticipant(request.OwnerId, request.ParticipantUserId);
        await _sessionRepository.UpdateAsync(session);

        _logger.LogInformation("Owner {OwnerId} removed user {ParticipantUserId} from session {SessionCode}",
            request.OwnerId, request.ParticipantUserId, request.SessionCode);
    }
}