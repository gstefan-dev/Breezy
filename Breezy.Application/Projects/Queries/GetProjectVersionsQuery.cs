using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Queries;

public record GetProjectVersionsQuery(Guid ProjectId, Guid UserId) : IRequest<IEnumerable<ProjectVersionResponse>>;