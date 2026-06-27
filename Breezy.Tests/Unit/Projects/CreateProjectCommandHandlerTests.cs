using Breezy.Application.Interfaces;
using Breezy.Application.Projects.Commands;
using Breezy.Domain.Projects.Entities;
using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.Projects;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly CreateProjectCommandHandler _handler;
    private readonly User _user;
    private readonly CreateProjectCommand _command;
    private readonly DawProject _project;

    public CreateProjectCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        _handler = new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object,
            _azureBlobServiceMock.Object,
            Mock.Of<ILogger<CreateProjectCommandHandler>>());
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestName" );
        _command = new CreateProjectCommand(_user.Id, "TestProjectName", "testLocalPath", "ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890ABCDEF1234567890",
            new MemoryStream(new byte[] { 1, 2, 3 }), "fileType", 2);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsProjectResponse()
    {
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _azureBlobServiceMock.Setup(x => x.GetUrlAsync(It.IsAny<string>()))
            .ReturnsAsync("Url");

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(_user.Id);
        result.ProjectName.Should().Be(_command.ProjectName);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ThrowsDomainException()
    {
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_WithNoAvailableStorage_ThrowsDomainException()
    {
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);
        _user.UseStorage(1024L * 1024 * 1024);
        
        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>().WithMessage("User dont have storage");
    }

    [Fact]
    public async Task Handle_WithNullUrl_ThrowsDomainException()
    {
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _azureBlobServiceMock.Setup(x => x.GetUrlAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);
        
        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>().WithMessage("Url is null");
    }

    
}