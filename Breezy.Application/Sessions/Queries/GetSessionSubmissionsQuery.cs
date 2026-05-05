namespace Breezy.Application.Sessions.Queries;

public record GetSessionSubmissionsQuery(string SessionCode, Guid OwnerId);