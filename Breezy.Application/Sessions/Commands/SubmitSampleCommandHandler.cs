using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Commands;

public class SubmitSampleCommandHandler : IRequestHandler<SubmitSampleCommand, SubmissionResponse>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAzureBlobService _azureBlobServiceRepo;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SubmitSampleCommandHandler> _logger;

    public SubmitSampleCommandHandler(ISessionRepository sessionRepository,
        IAzureBlobService azureBlobServiceRepo,
        IUserRepository userRepository,
        ILogger<SubmitSampleCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _azureBlobServiceRepo = azureBlobServiceRepo;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SubmissionResponse> Handle(SubmitSampleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} submitting file {FileName} to session {SessionCode}", 
            request.UserId, request.FileName, request.SessionCode);

        var sessionCode = new SessionCode(request.SessionCode);

        var session = await _sessionRepository.GetByCodeAsync(sessionCode);
        if (session == null)
        {
            _logger.LogWarning("Session not found for SessionCode: {SessionCode} ", sessionCode);
            throw new DomainException("Session not found");
        }
        
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found for UserId: {UserId}", request.UserId);
            throw new DomainException("User not found");
        }

        if (!user.HasAvailableStorage(request.FileSize))
        {
            _logger.LogWarning("User {UserId} has insufficient storage", request.UserId);
            throw new DomainException("Insufficient storage space");
        }
        var blobName = $"{request.UserId}/{Guid.NewGuid()}_{request.FileName}";
        await _azureBlobServiceRepo.UploadAsync(blobName, request.FileContent, request.ContentType);

        var url = await _azureBlobServiceRepo.GetUrlAsync(blobName);
        if (url == null)
            throw new DomainException("Failed to get file URL");

        var submissionMetadata = new SubmissionMetadata(request.FileName, request.DurationSeconds, request.BPM, request.Genre,
            request.Key, DateTime.UtcNow);

        var submission = new SessionSubmission(session.Id, user.Id, user.UserName, url, submissionMetadata);
        
        session.AddSubmission(submission);
        user.UseStorage(request.FileSize);
        
        await _sessionRepository.UpdateAsync(session);
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("File {FileName} submitted successfully to session {SessionCode} by user {UserId}", 
            request.FileName, request.SessionCode, request.UserId);

        return new SubmissionResponse(submission.Id, session.Id, user.Id, request.FileName, url, submission.CreatedAt);
    }
}