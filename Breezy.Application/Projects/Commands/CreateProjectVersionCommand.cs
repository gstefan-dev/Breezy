using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Commands;

public record CreateProjectVersionCommand(Guid ProjectId, Guid UserId, string NewHash, 
    Stream FileContent, string FileType, long FileSize) : IRequest<ProjectVersionResponse>;