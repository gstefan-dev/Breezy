using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Entities;

public class CollaborationSession : Entity
{
    public Guid OwnerUserId { get; private set; }
    public string OwnerName { get; private set; }
    public string Name { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public SessionCode Code { get; private set; }
    public int MaxParticipants { get; private set; }
    public bool IsClosedByOwner { get; private set; }
    public SessionVisibility Visibility { get; private set; }

    private readonly List<SessionParticipant> _participants = new();
    private readonly List<SessionSubmission> _submissions = new();

    public IReadOnlyList<SessionParticipant> Participants => _participants.AsReadOnly();

    public IReadOnlyList<SessionSubmission> Submissions => _submissions.AsReadOnly();
    
    private CollaborationSession() {}

    public CollaborationSession(
        Guid ownerUserId,
        string ownerName,
        SessionCode code,
        string name,
        DateTime? expiresAt = null,
        int maxParticipants = -1,
        SessionVisibility visibility = SessionVisibility.Curated) : base(Guid.NewGuid())
    {
        if (ownerUserId == Guid.Empty)
            throw new SessionDomainException("OwnerUserId cannot be empty", Id, ownerUserId);
        
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new SessionDomainException("OwnerName cannot be empty", Id, ownerUserId);

        if (string.IsNullOrWhiteSpace(name))
            throw new SessionDomainException("SessionName cannot be empty", Id, ownerUserId);

        if (expiresAt.HasValue && expiresAt <= DateTime.UtcNow)
            throw new SessionDomainException("Expiration date must be in the future", Id, ownerUserId);

        OwnerUserId = ownerUserId;
        OwnerName = ownerName.Trim();
        ExpiresAt = expiresAt;
        Name = name.Trim();
        MaxParticipants = maxParticipants;
        Code = code;
        IsClosedByOwner = false;
        Visibility = visibility;
        
        _participants.Add(new SessionParticipant(Id, ownerUserId, ownerName));

    }
    
    public static CollaborationSession Create(
        Guid ownerUserId,
        string ownerName,
        string name,
        int maxParticipants = -1,
        DateTime? expiresAt = null,
        SessionVisibility visibility = SessionVisibility.Curated)
    {
        var code = SessionCode.Generate();
        return new CollaborationSession(ownerUserId, ownerName, code, name, expiresAt, maxParticipants, visibility);
    }

    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    public bool IsActive => !IsExpired && !IsClosedByOwner;

    public void AddParticipant(Guid userId, string userName)
    {
        if (!IsActive)
            throw new SessionDomainException("Cannot add participant to an inactive session", Id, userId);

        if (MaxParticipants > 0 && _participants.Count(p => p.IsActive) >= MaxParticipants)
            throw new SessionDomainException($"Session has reached maximum participants ({MaxParticipants})", Id, userId);

        var existing = _participants.FirstOrDefault(p => p.UserId == userId);
        if (existing != null)
        {
            if (existing.IsActive)
                throw new SessionDomainException("User is already participant", Id, userId);

            if (existing.ExitReason == ParticipantExitReason.RemovedByOwner)
                throw new SessionDomainException("You were removed from this session by the owner", Id, userId);

            existing.Rejoin();
            existing.Restore();
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        _participants.Add(new SessionParticipant(Id, userId, userName));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSubmission(SessionSubmission submission)
    {
        if (submission.SessionId != Id)
            throw new SessionDomainException("Submissions does not belong to this session", Id);

        if (!IsActive)
            throw new SessionDomainException("Cannot add submissions to an inactive session", Id);

        if (!_participants.Any(p => p.UserId == submission.SubmittedByUserId && p.IsActive))
            throw new SessionDomainException("User is not an active participant of this session", Id, submission.SubmittedByUserId);

        _submissions.Add(submission);
        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseByOwner(Guid requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            throw new SessionDomainException("Only session owner can close the session", Id, requestingUserId);

        IsClosedByOwner = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen(Guid requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            throw new SessionDomainException("Only session owner can reopen the session", Id, requestingUserId);

        if (!IsClosedByOwner)
            throw new SessionDomainException("Session in not manually closed", Id, requestingUserId);

        IsClosedByOwner = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExpiration(Guid requestingUserId, DateTime? expiresAt)
    {
        if (requestingUserId != OwnerUserId)
            throw new SessionDomainException("Only session owner can set expiration ", Id, requestingUserId);

        if (expiresAt.HasValue && expiresAt <= DateTime.UtcNow)
            throw new SessionDomainException("Expiration date must be in the future", Id, requestingUserId);

        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public IReadOnlyList<SessionSubmission> GetVisibleSubmissions(Guid requestingUserId)
    {
        if (Visibility == SessionVisibility.Collaborative)
        {
            if (!_participants.Any(p => p.UserId == requestingUserId && p.IsActive))
                throw new SessionDomainException("Only active participants can view submissions", Id, requestingUserId);

            return _submissions.AsReadOnly();
        }

        if (requestingUserId != OwnerUserId)
            throw new SessionDomainException("Only session owner can view submissions", Id, requestingUserId);

        return _submissions.AsReadOnly();
    }
    
    public void Leave(Guid userId)
    {
        if (userId == OwnerUserId)
            throw new SessionDomainException("Owner cannot leave the session. Close it instead.", Id, userId);

        var participant = _participants.FirstOrDefault(p => p.UserId == userId && p.IsActive)
                          ?? throw new SessionDomainException("User is not an active participant of this session", Id, userId);

        participant.Leave();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveParticipant(Guid requestingUserId, Guid participantUserId)
    {
        if (requestingUserId != OwnerUserId)
            throw new SessionDomainException("Only session owner can remove participants", Id, requestingUserId);

        if (participantUserId == OwnerUserId)
            throw new SessionDomainException("Owner cannot remove themselves", Id, requestingUserId);

        var participant = _participants.FirstOrDefault(p => p.UserId == participantUserId && p.IsActive)
                          ?? throw new SessionDomainException("User is not an active participant of this session", Id, participantUserId);

        participant.RemoveByOwner();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ArchiveForUser(Guid userId)
    {
        if (userId == OwnerUserId && IsActive)
            throw new SessionDomainException("Close the session before archiving it", Id, userId);

        var participant = _participants.FirstOrDefault(p => p.UserId == userId)
                          ?? throw new SessionDomainException("User was never part of this session", Id, userId);

        participant.Archive();
    }

    public void RestoreForUser(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId)
                          ?? throw new SessionDomainException("User was never part of this session", Id, userId);

        participant.Restore();
    }

    public string GetDisplayName() => $"{Name} (Code: {Code.Value})";

    public int GetParticipantCount() => _participants.Count(p => p.IsActive);

    public int GetSubmissionCount() => _submissions.Count;
}

