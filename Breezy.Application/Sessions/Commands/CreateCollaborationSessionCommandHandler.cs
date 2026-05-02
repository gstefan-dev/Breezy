using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;


namespace Breezy.Application.Sessions.Commands;

public class CreateCollaborationSessionCommandHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateCollaborationSessionCommandHandler> _logger;

    public CreateCollaborationSessionCommandHandler(ISessionRepository session, IUserRepository user, ILogger<CreateCollaborationSessionCommandHandler> logger)
    {
        _sessionRepository = session;
        _userRepository = user;
        _logger = logger;
    }

    public async Task<SessionResponse> Handle(CreateCollaborationSessionCommand command)
    {
        _logger.LogInformation("Creating session for HostId: {HostId}", command.HostId);
        var user = await _userRepository.GetUserByIdAsync(command.HostId);

        if (user == null)
        {
            _logger.LogWarning("User not found for HostId: {HostId}", command.HostId);
            throw new DomainException("Host user not found");
        }

        var session = CollaborationSession.Create(
            user.Id,
            user.UserName,
            command.SessionName,
            command.MaxParticipants);

        await _sessionRepository.AddAsync(session);
        _logger.LogInformation("Session created successfully with Code: {Code} for HostId: {HostId}", 
            session.Code.Value, command.HostId);
        
        return new SessionResponse(session.Id, session.Code.Value, session.Name, session.IsActive, session.OwnerUserId, session.CreatedAt);
    }
    
}