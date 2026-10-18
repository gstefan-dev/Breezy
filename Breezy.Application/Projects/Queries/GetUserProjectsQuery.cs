using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Queries;

public record GetUserProjectsQuery(Guid UserId) : IRequest<IEnumerable<ProjectResponse>>;
