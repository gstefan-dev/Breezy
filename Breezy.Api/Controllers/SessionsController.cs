using Breezy.Application.Sessions.Commands;
using Breezy.Application.Sessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Breezy.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        var command = new CreateCollaborationSessionCommand(CurrentUserId, request.SessionName, request.MaxParticipants, request.Visibility);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSession), new { code = result.Code }, result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult> GetSession(string code)
    {
        var result = await _mediator.Send(new GetSessionByCodeQuery(code));
        return Ok(result);
    }

    [HttpPost("{code}/participants")]
    public async Task<ActionResult> AddParticipant(string code)
    {
        var command = new AddParticipantCommand(CurrentUserId, code);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{code}/samples")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> SubmitSample(string code,
        [FromForm] string fileName,
        [FromForm] int durationSeconds,
        [FromForm] string key,
        [FromForm] string genre,
        [FromForm] int bpm,
        IFormFile file)
    {
        await using var stream = file.OpenReadStream();

        var command = new SubmitSampleCommand(
            code,
            CurrentUserId,
            fileName,
            file.Length,
            durationSeconds,
            key,
            genre,
            bpm,
            stream,
            file.ContentType
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{code}/close")]
    public async Task<ActionResult> CloseSession(string code)
    {
        var command = new CloseSessionCommand(CurrentUserId, code);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("{code}/reopen")]
    public async Task<ActionResult> ReopenSession(string code)
    {
        var command = new ReopenSessionCommand(CurrentUserId, code);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet("{code}/submissions")]
    public async Task<ActionResult> GetSubmissions(string code)
    {
        var query = new GetSessionSubmissionsQuery(code, CurrentUserId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
    
    [HttpGet("{code}/participants")]
    public async Task<ActionResult> GetParticipants(string code)
    {
        var result = await _mediator.Send(new GetSessionParticipantsQuery(code));
        return Ok(result);
    }
    
    [HttpGet("user")]
    public async Task<ActionResult> GetUserSessions()
    {
        var result = await _mediator.Send(new GetUserSessionsQuery(CurrentUserId));
        return Ok(result);
    }
}