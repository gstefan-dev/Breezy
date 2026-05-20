using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Queries;

public record GetProjectVersionQuery(Guid ProjectId, Guid UserId) : IRequest<ProjectVersionResponse>;