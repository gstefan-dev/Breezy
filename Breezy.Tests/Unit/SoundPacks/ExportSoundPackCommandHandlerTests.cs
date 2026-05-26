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

public class ExportSoundPackCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ISoundPackRepository> _soundPackRepoMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly ExportSoundPackCommandHandler _handler;
    private readonly ExportSoundPackCommand _command;
    private readonly User _user;
    private readonly SoundPack _soundPack;
    private readonly SoundPackFile _packFile;


    public ExportSoundPackCommandHandlerTests()
    {
        _soundPackRepoMock = new Mock<ISoundPackRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        _handler = new ExportSoundPackCommandHandler(_soundPackRepoMock.Object, _userRepoMock.Object,
            _azureBlobServiceMock.Object,
            Mock.Of<ILogger<ExportSoundPackCommandHandler>>());
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestUserName");
        _soundPack = SoundPack.Create(_user.Id, _user.UserName, "TestPackName", "TestDescription", "TestGenre",
            "TestMood", "TestCategory");
        _command = new ExportSoundPackCommand(_soundPack.Id, _user.Id, ExportFormatEnum.Rar);
        _packFile = SoundPackFile.Create(_soundPack.Id, "FileName", SoundCategoryEnum.Loop, "Url", 180, 120, "F#", 2);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSoundPackResponse()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _azureBlobServiceMock.Setup(x => x.DownloadAsync(It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));

        _azureBlobServiceMock.Setup(x => x.GetUrlAsync(It.IsAny<string>()))
            .ReturnsAsync("export-url");
        _soundPack.AddFile(_packFile);
        var result = await _handler.Handle(_command, CancellationToken.None);

        result.Should().NotBeNull();
        result.OwnerName.Should().Be(_soundPack.OwnerName);
        result.SoundPackName.Should().Be(_soundPack.PackName);
        result.PackCategory.Should().Be(_soundPack.PackCategory);
        result.CreatedAt.Should().Be(_soundPack.CreatedAt);
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

        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_WithNonOwner_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        var otherUser = User.Create(Email.From("test2@gmail.com"), "passwordHash", "TestName2");

        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(otherUser);

        var newCommand = new ExportSoundPackCommand(_soundPack.Id, otherUser.Id, ExportFormatEnum.Rar);

        Func<Task> act = () => _handler.Handle(newCommand, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>().WithMessage("User is not the soundpack's Owner");
    }

    [Fact]
    public async Task Handle_WithNoFilesToExport_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("SoundPack has no files to export");
    }

    [Fact]
    public async Task Handle_WithNullUrl_ThrowsDomainException()
    {
        _soundPackRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_soundPack);

        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        _azureBlobServiceMock.Setup(x => x.DownloadAsync(It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream(new byte[] { 1, 2, 3 }));

        _azureBlobServiceMock.Setup(x => x.GetUrlAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);
        
        _soundPack.AddFile(_packFile);
        
        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Failed to get export URL");
    }
}