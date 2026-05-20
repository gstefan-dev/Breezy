using Breezy.Application.Projects.Commands;
using Breezy.Application.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProject(Guid id, [FromQuery] Guid userId)
    {
        var query = new GetProjectQuery(id, userId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProject([FromBody] CreateProjectCommand request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    [HttpPost("{id}/versions")]
    public async Task<ActionResult> CreateNewVersion(Guid id, [FromBody] CreateProjectVersionCommand request)
    {
        var command = new CreateProjectVersionCommand(id, request.UserId, request.NewHash, request.FileContent,
            request.FileType, request.FileSize);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("{id}/versions/{versionNumber}/changelog")]
    public async Task<ActionResult> AddChangelog(Guid id, int versionNumber,
        [FromBody] AddChangelogToVersionCommand request)
    {
        var command = new AddChangelogToVersionCommand(id, request.UserId, versionNumber, request.Changelog);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("{id}/versions")]
    public async Task<ActionResult> GetAllVersions(Guid id, [FromQuery] Guid userId)
    {
        var query = new GetProjectVersionsQuery(id, userId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id}/versions/latest")]
    public async Task<ActionResult> GetLatestProjectVersion(Guid id, [FromQuery] Guid userId)
    {
        var query = new GetProjectVersionQuery(id, userId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}