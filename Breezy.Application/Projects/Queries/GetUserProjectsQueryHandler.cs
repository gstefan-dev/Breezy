using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using MediatR;

namespace Breezy.Application.Projects.Queries;

public class GetUserProjectsQueryHandler : IRequestHandler<GetUserProjectsQuery, IEnumerable<ProjectResponse>>
{
    private readonly IProjectRepository _projectRepository;

    public GetUserProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectResponse>> Handle(GetUserProjectsQuery query, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetByUserIdAsync(query.UserId);

        return projects
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectResponse(p.Id, p.OwnerUserId, p.ProjectName, p.CreatedAt));
    }
}