using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Commands;

public record CreateProjectCommand(Guid OwnerId, string ProjectName, string? LocalPath, 
    string Hash, Stream FileContent, string FileType, long FileSize) : IRequest<ProjectResponse> ;