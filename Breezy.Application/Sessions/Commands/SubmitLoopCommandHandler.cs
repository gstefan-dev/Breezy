using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class SubmitLoopCommandHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAzureBlobService _azureBlobServiceRepo;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SubmitLoopCommandHandler> _logger;

    public SubmitLoopCommandHandler(ISessionRepository sessionRepository,
        IAzureBlobService azureBlobServiceRepo,
        IUserRepository userRepository,
        ILogger<SubmitLoopCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _azureBlobServiceRepo = azureBlobServiceRepo;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SubmissionResponse> Handle(SubmitLoopCommand command)
    {
        _logger.LogInformation("User {UserId} submitting file {FileName} to session {SessionCode}", 
            command.UserId, command.FileName, command.SessionCode);

        var sessionCode = new SessionCode(command.SessionCode);

        var session = await _sessionRepository.GetByCodeAsync(sessionCode);
        if (session == null)
        {
            _logger.LogWarning("Session not found for SessionCode: {SessionCode} ", sessionCode);
            throw new DomainException("Session not found");
        }
        
        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found for UserId: {UserId}", command.UserId);
            throw new DomainException("User not found");
        }

        if (!user.HasAvailableStorage(command.FileSize))
        {
            _logger.LogWarning("User {UserId} has insufficient storage", command.UserId);
            throw new DomainException("Insufficient storage space");
        }
        var blobName = $"{command.UserId}/{Guid.NewGuid()}_{command.FileName}";
        await _azureBlobServiceRepo.UploadAsync(blobName, command.FileContent, command.ContentType);

        var url = await _azureBlobServiceRepo.GetUrlAsync(blobName);
        if (url == null)
            throw new DomainException("Failed to get file URL");

        var submissionMetadata = new SubmissionMetadata(command.FileName, command.DurationSeconds, command.BPM, command.Genre,
            command.Key, DateTime.UtcNow);

        var submission = new SessionSubmission(session.Id, user.Id, user.UserName, url, submissionMetadata);
        
        session.AddSubmission(submission);
        user.UseStorage(command.FileSize);
        
        await _sessionRepository.UpdateAsync(session);
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("File {FileName} submitted successfully to session {SessionCode} by user {UserId}", 
            command.FileName, command.SessionCode, command.UserId);

        return new SubmissionResponse(submission.Id, session.Id, user.Id, command.FileName, url, submission.CreatedAt);
    }
}