using Breezy.Domain.Shared;

namespace Breezy.Domain.Entities;

public class Loop
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public int? Bpm { get; private set; }
    public string? Key { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Loop(){ }

    public Loop(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Filename required");
        
        Id = Guid.NewGuid();
        FileName = fileName;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAnalysis(int bpm, int key)
    {
        if (bpm < 40 || bpm > 300)
            throw new DomainException(nameof(bpm));
    }
}