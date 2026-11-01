using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.Commands;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using Breezy.Domain.SoundPacks.ValueObjects;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.SoundPacks;

public class AddFileToSoundPackCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ISoundPackRepository> _soundPackRepoMock;
    private readonly Mock<IAzureBlobService> _azureBlobRepoMock;
    private readonly AddFileToSoundPackCommandHandler _handler;
    private readonly AddFileToSoundPackCommand _command;
    private readonly User _user;
    private readonly SoundPack _soundPack;

    public AddFileToSoundPackCommandHandlerTests()
    {
        _soundPackRepoMock = new Mock<ISoundPackRepository>();
        _azureBlobRepoMock = new Mock<IAzureBlobService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new AddFileToSoundPackCommandHandler(_soundPackRepoMock.Object, _azureBlobRepoMock.Object,
            Mock.Of<ILogger<AddFileToSoundPackCommandHandler>>(), _userRepositoryMock.Object);
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestUserName");
        _soundPack = SoundPack.Create(_user.Id, _user.UserName, "TestPackName", "TestDescription", "TestGenre",
            "TestMood", "TestCategory");
        _command = new AddFileToSoundPackCommand(
            _soundPack.Id, _user.Id, "test.mp3", SoundCategoryEnum.Loop,
            new MemoryStream(new byte[] { 1, 2, 3 }), "audio/mp3", 1024,
            120, 128, "C");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSoundPackFileResponse()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _azureBlobRepoMock.Setup(x => x.GetUrlAsync(It.IsAny<string>()))
            .ReturnsAsync("url");

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.Should().NotBeNull();
        result.FileName.Should().Be(_command.FileName);
        result.Category.Should().Be(_command.SoundCategory.ToString());
        // result.FileUrl.Should().Be("url");
    }

    [Fact]
    public async Task Handle_WithNonExistentSoundPack_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((SoundPack?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("SoundPack dont exist");
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_WithNonOwner_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        var otherUser = User.Create(Email.From("test2@gmail.com"), "passwordHash2", "TestUserName2");

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(otherUser);
        var otherCommand = new AddFileToSoundPackCommand(_soundPack.Id, otherUser.Id, "test.mp3",
            SoundCategoryEnum.Loop,
            new MemoryStream(new byte[] { 1, 2, 3 }), "audio/mp3", 1024,
            120, 128, "C");

        Func<Task> act = () => _handler.Handle(otherCommand, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("User is not the soundpack's Owner");
    }

    [Fact]
    public async Task Handle_WithNoAvailableStorage_ThrowsDomainException()
    {
        var poorUser = User.Create(Email.From("poor@gmail.com"), "passwordHash", "PoorUser");
        poorUser.UseStorage(1024L * 1024 * 1024);
        
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(SoundPack.Create(poorUser.Id, _user.UserName, "TestPackName", "TestDescription", "TestGenre",
                "TestMood", "TestCategory"));

        var otherCommand = new AddFileToSoundPackCommand(_soundPack.Id, poorUser.Id, "test.mp3", SoundCategoryEnum.Loop,
            new MemoryStream(new byte[] { 1, 2, 3 }), "audio/mp3", 1024,
            120, 128, "C");

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(poorUser);

        Func<Task> act = () => _handler.Handle(otherCommand, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("User dont have storage");
    }

    [Fact]
    public async Task Handle_WithNullUrl_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _azureBlobRepoMock.Setup(x => x.GetUrlAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);
        
        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Url doesnt exist");
    }
}