namespace Breezy.Application.Projects.Queries;

public record GetProjectVersionsQuery(Guid ProjectId, Guid UserId);