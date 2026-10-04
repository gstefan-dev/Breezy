using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Breezy.Application.SoundPacks.Commands;
using Breezy.Application.SoundPacks.Queries;
using Breezy.Domain.SoundPacks.ValueObjects;
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
    public async Task<ActionResult> CreateSoundPack([FromBody] CreateSoundPackRequest request)
    {
        var command = new CreateSoundPackCommand(CurrentUserId, request.PackName, request.Description, request.Mood, request.Genre, request.PackCategory);
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetSoundPack), new { id = result.Id }, result);
    }

    [HttpPost("{id}/files")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> AddFile(Guid id, [FromForm] string fileName,
        [FromForm] SoundCategoryEnum soundCategory,
        [FromForm] int durationSeconds, [FromForm] int bpm, [FromForm] string key, IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var command = new AddFileToSoundPackCommand(id, CurrentUserId, fileName, soundCategory, stream,
            file.ContentType, file.Length, durationSeconds, bpm, key);

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