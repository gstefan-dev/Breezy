using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Breezy.Application.SoundPacks.Commands;
using Breezy.Application.SoundPacks.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SoundPacksController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SoundPacksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSoundPack(Guid id)
    {
        var query = new GetSoundPackQuery(id, CurrentUserId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateSoundPack([FromBody] CreateSoundPackCommand request)
    {
        var command = request with { UserId = CurrentUserId };
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetSoundPack), new { id = result.Id }, result);
    }

    [HttpPost("{id}/files")]
    public async Task<ActionResult> AddFile(Guid id, [FromBody] AddFileToSoundPackCommand request)
    {
        var command = new AddFileToSoundPackCommand(id, CurrentUserId, request.FileName, request.SoundCategory,
            request.FileContent, request.FileType, request.FileSize, request.DurationSeconds, request.BPM, request.Key);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id}/files/{fileId}")]
    public async Task<ActionResult> RemoveFile(Guid id, Guid fileId, [FromBody] RemoveFileFromSoundPackCommand request)
    {
        var command = new RemoveFileFromSoundPackCommand(id, CurrentUserId, fileId, request.FileSize);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("{id}/export")]
    public async Task<ActionResult> ExportSoundPack(Guid id, [FromBody] ExportSoundPackCommand request)
    {
        var command = new ExportSoundPackCommand(id, CurrentUserId, request.ExportFormat);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("user")]
    public async Task<ActionResult> GetUserSoundPacks()
    {
        var result = await _mediator.Send(new GetUserSoundPacksQuery(CurrentUserId));
        return Ok(result);
    }
 }