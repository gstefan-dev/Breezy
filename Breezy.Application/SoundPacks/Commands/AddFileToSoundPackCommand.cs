using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.SoundPacks.ValueObjects;
using MediatR;

namespace Breezy.Application.SoundPacks.Commands;

public record AddFileToSoundPackCommand(Guid SoundPackId, Guid UserId, string FileName, SoundCategoryEnum SoundCategory,
    Stream FileContent, string FileType, long FileSize, 
    int DurationSeconds, int BPM, string Key) : IRequest<SoundPackFileResponse>;