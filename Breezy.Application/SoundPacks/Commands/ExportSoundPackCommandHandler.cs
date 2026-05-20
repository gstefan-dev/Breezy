using System.IO.Compression;
using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using Breezy.Domain.SoundPacks.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class ExportSoundPackCommandHandler : IRequestHandler<ExportSoundPackCommand, SoundPackResponse>
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<ExportSoundPackCommandHandler> _logger;

    public ExportSoundPackCommandHandler(ISoundPackRepository soundPackRepository, IUserRepository userRepository,
        IAzureBlobService azureBlobService, ILogger<ExportSoundPackCommandHandler> logger)
    {
        _soundPackRepository = soundPackRepository;
        _userRepository = userRepository;
        _azureBlobService = azureBlobService;
        _logger = logger;
    }

    public async Task<SoundPackResponse> Handle(ExportSoundPackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting SoundPack {SoundPackId} for UserId: {UserId}", request.SoundPackId, request.UserId);

        var soundPack = await _soundPackRepository.GetByIdAsync(request.SoundPackId);
        if (soundPack == null)
        {
            _logger.LogWarning("SoundPack {SoundPackId} not found", request.SoundPackId);
            throw new DomainException("SoundPack dont exist");
        }

        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", request.UserId);
            throw new DomainException("User not found");
        }

        if (soundPack.OwnerUserId != user.Id)
        {
            _logger.LogWarning("User {UserId} is not owner of SoundPack {SoundPackId}", request.UserId,
                request.SoundPackId);
            throw new DomainException("User is not the soundpack's Owner");
        }

        if (!soundPack.IsReadyToExport())
        {
            _logger.LogWarning("SoundPack {SoundPackId} has no files to export", request.SoundPackId);
            throw new DomainException("SoundPack has no files to export");
        }

        var blobName = $"{request.UserId}/exports/{Guid.NewGuid()}_{soundPack.PackName}";

        var contentType = request.ExportFormat switch
        {
            ExportFormatEnum.Zip => "application/zip",
            ExportFormatEnum.Rar => "application/x-rar-compressed",
            ExportFormatEnum.Tar => "application/x-tar",
            _ => "application/zip"
        };

        var archiveStream = new MemoryStream();
        using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in soundPack.Files)
            {
                var fileStream = await _azureBlobService.DownloadAsync(file.BlobUrl);
                var entry = archive.CreateEntry(file.FileName);
                using var entryStream = entry.Open();
                await fileStream.CopyToAsync(entryStream);
            }
        }

        archiveStream.Position = 0;

        await _azureBlobService.UploadAsync(blobName, archiveStream, contentType);

        var url = await _azureBlobService.GetUrlAsync(blobName);
        if (url == null)
        {
            _logger.LogWarning("Failed to get URL for export {BlobName}", blobName);
            throw new DomainException("Failed to get export URL");
        }

        var export = new SoundPackExport(soundPack.Id, request.ExportFormat, url);
        soundPack.SetExport(export);

        await _soundPackRepository.UpdateAsync(soundPack);

        _logger.LogInformation("SoundPack {SoundPackId} exported successfully", request.SoundPackId);

        return new SoundPackResponse(soundPack.Id, soundPack.PackName, soundPack.OwnerName,
            soundPack.Genre, soundPack.PackCategory, soundPack.CreatedAt);
    }
}