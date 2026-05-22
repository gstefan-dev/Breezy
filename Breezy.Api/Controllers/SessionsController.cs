using Breezy.Application.Sessions.Commands;
using Breezy.Application.Sessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateSession([FromBody] CreateCollaborationSessionCommand request)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetSession), new { code = result.Code }, result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult> GetSession(string code)
    {
        var result = await _mediator.Send(new GetSessionByCodeQuery(code));
        return Ok(result);
    }

    [HttpPost("{code}/participants")]
    public async Task<ActionResult> AddParticipant(string code, [FromBody] AddParticipantCommand request)
    {
        var command = new AddParticipantCommand(request.UserId, code);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{code}/samples")]
    public async Task<ActionResult> SubmitSample(string code, [FromBody] SubmitSampleCommand request)
    {
        var command = new SubmitSampleCommand(code, request.UserId, request.FileName, request.FileSize,
            request.DurationSeconds, request.Key, request.Genre, request.BPM, request.FileContent, request.ContentType);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{code}/close")]
    public async Task<ActionResult> CloseSession(string code, [FromBody] CloseSessionCommand request)
    {
        var command = new CloseSessionCommand(request.UserId, code);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("{code}/reopen")]
    public async Task<ActionResult> ReopenSession(string code, [FromBody] ReopenSessionCommand request)
    {
        var command = new ReopenSessionCommand(request.UserId, code);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("{code}/submissions")]
    public async Task<ActionResult> GetSubmissions(string code, [FromQuery] Guid ownerId)
    {
        var query = new GetSessionSubmissionsQuery(code, ownerId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}