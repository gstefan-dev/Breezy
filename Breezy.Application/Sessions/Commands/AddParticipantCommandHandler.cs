using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class AddParticipantCommandHandler : IRequestHandler<AddParticipantCommand, ParticipantResponse>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AddParticipantCommandHandler> _logger;
    

    public AddParticipantCommandHandler(ISessionRepository sessionRepository, IUserRepository userRepository, ILogger<AddParticipantCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ParticipantResponse> Handle(AddParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Joining new participant at session: {SessionCode}", request.SessionCode);

        var code = new SessionCode(request.SessionCode);
        var session = await _sessionRepository.GetByCodeAsync(code);

        if (session == null)
        {
            _logger.LogWarning("Session not found for SessionCode: {SessionCode} ", code);
            throw new DomainException("Session not found");
        }

        var user = await _userRepository.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            _logger.LogWarning("User not found for SessionCode: {SessionCode}", code);
            throw new DomainException("User not found");
        }
        
        session.AddParticipant(user.Id, user.UserName);
        await _sessionRepository.UpdateAsync(session);
        
        _logger.LogInformation("Participant {UserName} was added succesfully to session {SessionCode}", user.UserName, code);

        var participant = session.Participants.Last();
        return new ParticipantResponse(participant.Id, participant.SessionId,participant.UserId, participant.UserName, participant.JoinedAt);
    }
}