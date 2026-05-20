using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Queries;

public record GetSessionByCodeQuery(string SessionCode) : IRequest<SessionResponse>;