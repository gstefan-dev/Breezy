using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.ValueObjects;

public class SubmissionMetadata : ValueObject
{
    public string FileName { get; }
    public int DurationSeconds { get; }
    public int BPM { get; }
    public string Genre { get; }
    public string Key { get; }
    public DateTime SubmittedAt { get; }
    
    private SubmissionMetadata (){}

    public SubmissionMetadata(
        string fileName,
        int durationSeconds,
        int bpm,
        string genre,
        string key,
        DateTime submittedAt)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidSubmissionException("File name cannot be empty", nameof(fileName));

        if (durationSeconds <= 0)
            throw new InvalidSubmissionException("Duration must be greater than 0", nameof(durationSeconds));

        if (bpm < 40 || bpm > 300)
            throw new InvalidSubmissionException($"Bpm must be between 40 and 300, Provided: {bpm}", nameof(bpm));

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidSubmissionException("Key cannot be empty", nameof(key));

        if (submittedAt > DateTime.UtcNow)
            throw new InvalidSubmissionException("Submission date cannot be in the future", nameof(submittedAt));

        FileName = fileName.Trim();
        DurationSeconds = durationSeconds;
        BPM = bpm;
        Key = key.Trim();
        Genre = !string.IsNullOrWhiteSpace(genre) ? genre.Trim() : "Unknown";
        SubmittedAt = submittedAt;

    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return FileName;
        yield return DurationSeconds;
        yield return BPM;
        yield return Key;
        yield return Genre;
        yield return SubmittedAt;
    }
}