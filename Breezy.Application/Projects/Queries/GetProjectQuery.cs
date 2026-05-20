using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Queries;

public record GetProjectQuery(Guid ProjectId, Guid UserId) : IRequest<ProjectResponse>;