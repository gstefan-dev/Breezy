using System.Runtime.CompilerServices;
using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.Commands;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.Sessions;

public class CloseSessionCommandHandlerTests
{
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly CloseSessionCommandHandler _handler;
    private readonly User _user;
    private readonly CloseSessionCommand _command;
    private readonly CollaborationSession _session;

    public CloseSessionCommandHandlerTests()
    {
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _handler = new CloseSessionCommandHandler(_sessionRepositoryMock.Object,
            Mock.Of<ILogger<CloseSessionCommandHandler>>());
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestUserName");
        _command = new CloseSessionCommand(_user.Id, "codeSes");
        _session = CollaborationSession.Create(_user.Id, _user.UserName, "sessionName", 4,
            DateTime.UtcNow.AddHours(1));
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSessionResponse()
    {
        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync(_session);

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.IsActive.Should().Be(false);
    }

    [Fact]
    public async Task Handle_WithNonExistentSession_ThrowsDomainException()
    {
        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync((CollaborationSession?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Session not found");
    }

    [Fact]
    public async Task Handle_WithNonOwner_ThrowsDomainException()
    {
        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync(_session);

        var otherUser = User.Create(Email.From("test2@gmail.com"), "passwordHash2", "TestUserName2");

        var otherCommand = new CloseSessionCommand(otherUser.Id, "codeSes");

        Func<Task> act = () => _handler.Handle(otherCommand, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Only session owner can close the session");
    }
    
    [Fact]
    public async Task Handle_WithValidCommand_SessionIsInactive()
    {
        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync(_session);

        await _handler.Handle(_command, CancellationToken.None);

        _session.IsActive.Should().Be(false);
    }
}