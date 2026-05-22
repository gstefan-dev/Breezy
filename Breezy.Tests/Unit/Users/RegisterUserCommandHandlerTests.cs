using Breezy.Application.Interfaces;
using Breezy.Application.Users.Commands;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.Users;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object, Mock.Of<ILogger<RegisterUserCommandHandler>>(),
            _authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsUserResponse()
    {
        var command = new RegisterUserCommand("TestUser", "test@gmail.com", "password123");

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync((User?)null);

        _authServiceMock
            .Setup(x => x.HashPasswordAsync(It.IsAny<string>()))
            .ReturnsAsync("hashedPassword");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("TestUser", result.UserName);
        Assert.Equal("test@gmail.com", result.Email);
        _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(It.IsAny<Email>()), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        
    }
    
    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsDomainException()
    {
        
        var command = new RegisterUserCommand("TestUser", "existing@gmail.com", "password123");

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(User.Create(Email.From("existing@gmail.com"), "passwordHash", "TestUser")); 
        
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Email already in use");
    
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ThrowsException()
    {
        var command = new RegisterUserCommand("TestUser", "notanemail", "password123");

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

}