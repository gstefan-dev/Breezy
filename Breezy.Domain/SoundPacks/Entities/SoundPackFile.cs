using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.ValueObjects;

namespace Breezy.Domain.SoundPacks.Entities;

public class SoundPackFile : Entity
{
    public Guid SoundPackId { get; private set; }
    public string FileName { get; private set; }
    public SoundCategoryEnum Category { get; private set; }
    public string BlobUrl { get; private set; }
    public int DurationInSeconds { get; private set; }
    public int Bpm { get; private set; }
    public string Key { get; private set; }
    
    private SoundPackFile(){}
    
    public SoundPackFile(Guid soundPackId, string fileName, SoundCategoryEnum category, string blobUrl, int durationInSeconds, int bpm, string key) : base(Guid.NewGuid())
    {
        if (soundPackId == Guid.Empty)
            throw new DomainException("SoundPackId cannot be empty");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("FileName cannot be empty");

        if (string.IsNullOrWhiteSpace(blobUrl))
        {
            throw new DomainException("BlobUrl cannot be empty");
        }
        
        if (durationInSeconds <= 0)
            throw new DomainException("DurationInSeconds cannot be less than or equal to zero");

        if (bpm <= 0)
            throw new DomainException("Bpm cannot be less than or equal to zero");
        
        SoundPackId = soundPackId;
        FileName = fileName;
        Category = category;
        BlobUrl = blobUrl;
        DurationInSeconds = durationInSeconds;
        Bpm = bpm;
        Key = !string.IsNullOrWhiteSpace(key) ? key.Trim() : "Unknown";
    }    
    
    public string GetDisplayName() => $"{FileName} ({Bpm}BPM, {Key})";
}