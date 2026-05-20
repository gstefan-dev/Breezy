using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Commands;

public record AddChangelogToVersionCommand(Guid ProjectId, Guid UserId, 
    int VersionNumber, string Changelog) : IRequest<ProjectVersionResponse>;