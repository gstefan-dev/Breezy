namespace Breezy.Application.Sessions.DTOs;

public record SessionResponse(Guid Id, string Code, string Name, bool IsActive, Guid HostId, DateTime CreatedAt);
public record SubmissionResponse(Guid Id, Guid SessionId, Guid SubmitterId, string FileName, string FileUrl, DateTime CreatedAt);
public record ParticipantResponse(Guid Id, Guid SessionId, Guid UserId, string UserName, DateTime JoinedAt);

