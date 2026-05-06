namespace Breezy.Application.Projects.Queries;

public record GetProjectVersionQuery(Guid ProjectId, Guid UserId);