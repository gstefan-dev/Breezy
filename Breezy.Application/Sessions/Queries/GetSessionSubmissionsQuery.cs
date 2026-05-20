using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Queries;

public record GetSessionSubmissionsQuery(string SessionCode, Guid OwnerId) : IRequest< IEnumerable<SubmissionResponse>>;