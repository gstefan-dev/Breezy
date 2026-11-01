using Breezy.Domain.Sessions.ValueObjects;

namespace Breezy.Application.Sessions.DTOs;

public record SessionResponse(Guid Id, string Code, string Name, bool IsActive, Guid HostId, DateTime CreatedAt, SessionVisibility Visibility);
public record SubmissionResponse(Guid Id, Guid SessionId, Guid SubmitterId, string FileName, string FileUrl, DateTime CreatedAt);
public record UserSessionResponse(Guid Id, string Code, string Name, bool IsActive, Guid HostId,
    DateTime CreatedAt, SessionVisibility Visibility, string MyStatus, bool IsArchived);

public record ParticipantResponse(Guid Id, Guid SessionId, Guid UserId, string UserName,
    DateTime JoinedAt, DateTime? LeftAt, string? ExitReason);

