using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.Commands;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Breezy.Tests.Unit.SoundPacks;

public class CreateSoundPackCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ISoundPackRepository> _soundPackRepoMock;
    private readonly CreateSoundPackCommandHandler _handler;
    private readonly CreateSoundPackCommand _command;
    private readonly User _user;

    public CreateSoundPackCommandHandlerTests()
    {
        _soundPackRepoMock = new Mock<ISoundPackRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _handler = new CreateSoundPackCommandHandler(_soundPackRepoMock.Object, _userRepoMock.Object,
            Mock.Of<ILogger<CreateSoundPackCommandHandler>>());
        _user = User.Create(Email.From("test@gmail.com"), "passwordHash", "TestUserName");
        _command = new CreateSoundPackCommand(_user.Id, "TestPackName", "TestDescription", "TestMood", "TestGenre",
            "TestCategory");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSoundPackResponse()
    {
        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.OwnerName.Should().Be(_user.UserName);
        result.PackCategory.Should().Be(_command.PackCategory);
        result.SoundPackName.Should().Be(_command.PackName);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ThrowsDomainException()
    {
        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        Func<Task> act = () => _handler.Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_WithValidCommand_SoundPackIsCreated()
    {
        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_user);
        
        SoundPack? createdSoundPack = null;
        _soundPackRepoMock.Setup(x => x.AddAsync(It.IsAny<SoundPack>()))
            .Callback<SoundPack>(soundPack => { createdSoundPack = soundPack; }).Returns(Task.CompletedTask);

        await _handler.Handle(_command, CancellationToken.None);

        createdSoundPack.Should().NotBeNull();
        createdSoundPack.PackName.Should().Be(_command.PackName);
        createdSoundPack.OwnerName.Should().Be(_user.UserName);
    }
    
}