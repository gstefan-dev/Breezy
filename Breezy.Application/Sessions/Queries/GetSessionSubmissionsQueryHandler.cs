using Breezy.Application.Interfaces;
using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Sessions.Queries;

public class GetSessionSubmissionsQueryHandler : IRequestHandler<GetSessionSubmissionsQuery, IEnumerable<SubmissionResponse>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<GetSessionSubmissionsQueryHandler> _logger;

    public GetSessionSubmissionsQueryHandler(ISessionRepository sessionRepository,
        ILogger<GetSessionSubmissionsQueryHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SubmissionResponse>> Handle(GetSessionSubmissionsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all submissions from {SessionCode} session", query.SessionCode);
        var sessionCode = new SessionCode(query.SessionCode);

        var session = await _sessionRepository.GetByCodeAsync(sessionCode);

        if (session == null)
        {
            _logger.LogWarning("Session with {SessionCode} code not found", sessionCode);
            throw new DomainException("Session not found");
        }

        var submissions = session.GetVisibleSubmissions(query.OwnerId);
        
        _logger.LogInformation("Found {Count} submissions for session {SessionCode}", 
            submissions.Count, query.SessionCode);
        return submissions.Select(submission => new SubmissionResponse(
            submission.Id,
            submission.SessionId,
            submission.SubmittedByUserId,
            submission.Metadata.FileName,
            submission.BlobUrl,
            submission.CreatedAt));
    }
}