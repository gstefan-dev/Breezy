using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.Commands;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.Sessions;

public class AddParticipantCommandTests
{
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AddParticipantCommandHandler _handler;
    private readonly User _user;
    private readonly AddParticipantCommand _command;

    public AddParticipantCommandTests()
    {
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new AddParticipantCommandHandler(_sessionRepositoryMock.Object, _userRepositoryMock.Object,
            Mock.Of<ILogger<AddParticipantCommandHandler>>());
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestUserName");
        _command = new AddParticipantCommand(Guid.NewGuid(), "code");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsParticipantResponse()
    {
        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync(CollaborationSession.Create(Guid.NewGuid(), _user.UserName, "sessionName", 4,
                DateTime.UtcNow.AddHours(1)));

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserName.Should().Be(_user.UserName);
        result.UserId.Should().Be(_user.Id);
        result.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
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
    public async Task Handle_WithNonExistentUser_ThrowsDomainException()
    {
        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync(CollaborationSession.Create(Guid.NewGuid(), _user.UserName, "sessionName", 4,
                DateTime.UtcNow.AddHours(1)));

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_WithInactiveSession_ThrowsDomainException()
    {
        var ownerId = Guid.NewGuid();
        var session = CollaborationSession.Create(ownerId, _user.UserName, "sessionName", 4,
            DateTime.UtcNow.AddHours(1));
        session.CloseByOwner(ownerId);

        _sessionRepositoryMock.Setup(x => x.GetByCodeAsync(It.IsAny<SessionCode>()))
            .ReturnsAsync(session);
        
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<SessionDomainException>()
            .WithMessage("Cannot add participant to an inactive session");
    }
}
