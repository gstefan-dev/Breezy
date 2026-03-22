using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Entities;

public class SessionSubmission : Entity
{
    public Guid SessionId { get; private set; }
    public Guid SubmittedByUserId { get; private set; }
    public string SubmitterName { get; private set; }
    public string BlobUrl { get; private set; }
    public SubmissionMetadata Metadata { get; private set; }
    
    private SessionSubmission(){}

    public SessionSubmission(
        Guid sessionId,
        Guid submittedByUserId,
        string submitterName,
        string blobUrl,
        SubmissionMetadata metadata) : base(Guid.NewGuid())
    {
        if (sessionId == Guid.Empty)
            throw new InvalidSubmissionException("SessionId cannot be empty", nameof(sessionId));

        if (submittedByUserId == Guid.Empty)
            throw new InvalidSubmissionException("SubmittedUserId cannot be empty", nameof(submittedByUserId));

        if (string.IsNullOrWhiteSpace(submitterName))
            throw new InvalidSubmissionException("SubmitterName cannot be empty", nameof(submitterName));

        if (string.IsNullOrWhiteSpace(blobUrl))
            throw new InvalidSubmissionException("BlobUrl cannot be empty", nameof(blobUrl));

        if (metadata == null)
            throw new InvalidSubmissionException("Metadata cannot be null", nameof(metadata));

        SessionId = sessionId;
        SubmittedByUserId = submittedByUserId;
        SubmitterName = submitterName.Trim();
        BlobUrl = blobUrl.Trim();
        Metadata = metadata;


    }

    public string GetDisplayName() => $"{Metadata.FileName} by {SubmitterName}";
}