using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.Commands;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.Sessions;

public class CreateCollaborationSessionCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly CreateCollaborationSessionCommandHandler _handler;
    private readonly User _user;

    public CreateCollaborationSessionCommandHandlerTests()
    {
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateCollaborationSessionCommandHandler(_sessionRepositoryMock.Object,
            _userRepositoryMock.Object, Mock.Of<ILogger<CreateCollaborationSessionCommandHandler>>());
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestUserName");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSessionResponse()
    {
        var command = new CreateCollaborationSessionCommand(
            Guid.NewGuid(),
            "TestSession",
            3);
        
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _sessionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CollaborationSession>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        result.Name.Should().Be(command.SessionName);
        _sessionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<CollaborationSession>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentHost_ThrowsDomainException()
    {
        var command = new CreateCollaborationSessionCommand(Guid.NewGuid(), "TestName", 4);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Host user not found");
    }

    [Fact]
    public async Task Handle_WithValidCommand_SessionIsActive()
    {
        var command = new CreateCollaborationSessionCommand(Guid.NewGuid(), "TestName", 4);
        CollaborationSession? createdSession = null;
        
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _sessionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<CollaborationSession>()))
            .Callback<CollaborationSession>(session => { createdSession = session; }).Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        createdSession.Should().NotBeNull();
        createdSession!.IsActive.Should().BeTrue();
        createdSession!.Name.Should().Be("TestName");
        createdSession!.MaxParticipants.Should().Be(4);
    }
}