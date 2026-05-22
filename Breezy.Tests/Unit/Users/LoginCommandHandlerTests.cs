using Breezy.Application.Interfaces;
using Breezy.Application.Users.Commands;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.Users;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new LoginCommandHandler(_userRepositoryMock.Object,
            _authServiceMock.Object, Mock.Of<ILogger<LoginCommandHandler>>());
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        var command = new LoginCommand("test@gmail.com", "password123");

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(User.Create(
                Email.From("test@gmail.com"),
                "hashedPassword",
                "TestUser"
            ));
        _authServiceMock.Setup(x => x.VerifyPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _authServiceMock.Setup(x => x.GenerateTokenAsync(It.IsAny<User>()))
            .ReturnsAsync("jwt_token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);

        result.Should().Be("jwt_token");
        _authServiceMock.Verify(x => x.GenerateTokenAsync(It.IsAny<User>()), Times.Once);

    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ThrowsDomainException()
    {
        var command = new LoginCommand("test@gmail.com", "password123");

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsDomainException()
    {
        var command = new LoginCommand("test@gmail.com", "password123");

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(User.Create(Email.From("test@gmail.com"), "password1234", "Test"));

        _authServiceMock.Setup(x => x.VerifyPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        Func<Task> act = () =>  _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid credentials");
    }
    
    
    
    
    
    
}