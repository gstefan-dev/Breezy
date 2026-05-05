namespace Breezy.Application.Projects.DTOs;

public record ProjectResponse(Guid Id, Guid UserId, string ProjectName, DateTime CreatedAt);
public record ProjectVersionResponse(Guid Id, Guid ProjectId, int VersionNumber, string Description, DateTime CreatedAt);
