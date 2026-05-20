using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record SubmitSampleCommand(string SessionCode, 
    Guid UserId, 
    string FileName, 
    long FileSize, 
    int DurationSeconds, 
    string Key, 
    string Genre, 
    int BPM,
    Stream FileContent,
    string ContentType) : IRequest<SubmissionResponse>;