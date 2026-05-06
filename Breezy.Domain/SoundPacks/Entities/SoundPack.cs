using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.ValueObjects;

namespace Breezy.Domain.SoundPacks.Entities;

public class SoundPack : Entity
{
    public Guid OwnerUserId { get; private set; }
    public string OwnerName { get; private set; }
    public string PackName { get; private set; }
    public string Description { get; private set; }
    public string Genre { get; private set; }
    public string Mood { get; private set; }
    
    public string PackCategory { get; private set; }
    
    private readonly List<SoundPackFile> _files = new();
    private SoundPackExport _lastExport;
    
    public IReadOnlyCollection<SoundPackFile> Files => _files.AsReadOnly();
    public SoundPackExport LastExport => _lastExport;
    
    private SoundPack(){}
    
    public SoundPack(Guid ownerUserId, string ownerName, string packName, string description = "", string genre = "Unknown", string mood = "Unknown", string packCategory = "General") : base(Guid.NewGuid())
    {
        if (ownerUserId == Guid.Empty)
            throw new DomainException("OwnerUserId cannot be empty");

        if (string.IsNullOrWhiteSpace(ownerName))
            throw new DomainException("OwnerName cannot be empty");

        if (string.IsNullOrWhiteSpace(packName))
            throw new DomainException("PackName cannot be empty");

        OwnerUserId = ownerUserId;
        OwnerName = ownerName;
        PackName = packName;
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : string.Empty;
        Genre = !string.IsNullOrWhiteSpace(genre) ? genre.Trim() : string.Empty;
        Mood = !string.IsNullOrWhiteSpace(mood) ? mood.Trim() : string.Empty;
        PackCategory = !string.IsNullOrWhiteSpace(packCategory) ? packCategory.Trim() : "General";
    }

    public static SoundPack Create(Guid ownerUserId, string ownerName, string packName, string description = "", string genre = "Unknown", string mood = "Unknown", string packCategory = "General")
    {
        return new SoundPack(ownerUserId, ownerName, packName, description, genre, mood, packCategory);
    }
    public void AddFile(SoundPackFile file)
    {
        if (file.SoundPackId != Id)
            throw new DomainException("File does not belong to this SoundPack");
        
        if (_files.Any(f => f.FileName == file.FileName))
            throw new DomainException($"File '{file.FileName}' already exists in this SoundPack");
        
        _files.Add(file);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void RemoveFile(Guid fileId)
    {
        if (_files.Count == 1)
            throw new DomainException("Pack must contain at least one file");

        var file = _files.FirstOrDefault(f => f.Id == fileId);
        if (file == null)
            throw new DomainException("File not found in pack");

        _files.Remove(file);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetExport(SoundPackExport export)
    {
        if (export.SoundPackId != Id)
            throw new DomainException("Export does not belong to this pack");

        _lastExport = export;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public IReadOnlyList<SoundPackFile> GetFilesByCategory(SoundCategoryEnum category)
    {
        return _files
            .Where(f => f.Category == category)
            .ToList()
            .AsReadOnly();
    }
    
    public int GetTotalDuration()
    {
        return _files.Sum(f => f.DurationInSeconds);
    }
    
    public bool IsReadyToExport()
    {
        return _files.Count > 0;
    }
    
    public bool HasExport()
    {
        return _lastExport != null;
    }
    
    public string GetDisplayName() => $"{PackName} by {OwnerName} ({_files.Count} files)";
    
    public int GetFileCountByCategory(SoundCategoryEnum category)
    {
        return _files.Count(f => f.Category == category);
    }
    
    public long GetEstimatedTotalSize()
    {
        // Rough estimate: 1 second ≈ 1 MB audio
        return GetTotalDuration() * 1024 * 1024;
    }
}