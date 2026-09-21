using Breezy.Application.Projects.Commands;
using Breezy.Application.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProject(Guid id)
    {
        var query = new GetProjectQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> CreateProject(
        [FromForm] string projectName,
        [FromForm] string hash,
        IFormFile file)
    {
        try
        {
            await using var stream = file.OpenReadStream();
        
            var command = new CreateProjectCommand(
                CurrentUserId,
                projectName,
                null,
                hash,
                stream,
                file.ContentType,
                file.Length
            );
        
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, innerError = ex.InnerException?.Message });
        }
    }

    [HttpPost("{id}/versions")]
    public async Task<ActionResult> CreateNewVersion(Guid id, [FromBody] CreateProjectVersionCommand request)
    {
        var command = new CreateProjectVersionCommand(id, CurrentUserId, request.NewHash, request.FileContent,
            request.FileType, request.FileSize);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("{id}/versions/{versionNumber}/changelog")]
    public async Task<ActionResult> AddChangelog(Guid id, int versionNumber,
        [FromBody] AddChangelogToVersionCommand request)
    {
        var command = new AddChangelogToVersionCommand(id, CurrentUserId, versionNumber, request.Changelog);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("{id}/versions")]
    public async Task<ActionResult> GetAllVersions(Guid id)
    {
        var query = new GetProjectVersionsQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id}/versions/latest")]
    public async Task<ActionResult> GetLatestProjectVersion(Guid id)
    {
        var query = new GetProjectVersionQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}