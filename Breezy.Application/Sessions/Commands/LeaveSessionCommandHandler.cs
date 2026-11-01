using Breezy.Application.Interfaces;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public class LeaveSessionCommandHandler : IRequestHandler<LeaveSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;

    public LeaveSessionCommandHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task Handle(LeaveSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByCodeAsync(new SessionCode(request.SessionCode))
                      ?? throw new DomainException("Session not found");

        session.Leave(request.UserId);
        await _sessionRepository.UpdateAsync(session);
    }
}