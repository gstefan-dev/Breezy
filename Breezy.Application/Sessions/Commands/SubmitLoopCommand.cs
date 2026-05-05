namespace Breezy.Application.Sessions.Commands;

public record SubmitLoopCommand(string SessionCode, 
    Guid UserId, 
    string FileName, 
    long FileSize, 
    int DurationSeconds, 
    string Key, 
    string Genre, 
    int BPM,
    Stream FileContent,
    string ContentType);