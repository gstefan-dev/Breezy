using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Projects.Entities;

public class DawProject : Entity
{
    public Guid OwnerUserId { get; private set; }
    public string ProjectName { get; private set; }
    public string LocalPath { get; private set; }
    public ProjectHash CurrentHash { get; private set; }
    public int CurrentVersionNumber { get; private set; }

    private readonly List<ProjectVersion> _versions = new();
    public IReadOnlyList<ProjectVersion> Versions => _versions;
    
    private DawProject() { }

    public DawProject(
        Guid ownerUserId,
        string projectName,
        string localPath,
        ProjectHash initialHash,
        int currentVersionNumber,
        string blobUrl) : base(Guid.NewGuid())
    {
        if (ownerUserId == Guid.Empty)
            throw new DomainException("OwnerUserId cannot be empty");

        if (string.IsNullOrWhiteSpace(projectName))
            throw new DomainException("ProjectName cannot be empty");

        if (string.IsNullOrWhiteSpace(localPath))
            throw new DomainException("LocalPath cannot be empty");

        if (initialHash == null)
            throw new DomainException("InitialHash cannot be empty");

        if (string.IsNullOrWhiteSpace(blobUrl))
            throw new DomainException("BlobUrl cannot be empty");

        var initialVersion = new ProjectVersion(Id, 1, initialHash, blobUrl);
        _versions.Add(initialVersion);

    }

    public void CreateNewVersionIfChanged(ProjectHash newHash, string blobUrl)
    {
        if (newHash == null)
            throw new DomainException("NewHash cannot be null");

        if (newHash == CurrentHash)
            throw new DomainException("Hash noot changed - new version not needed");

        if (string.IsNullOrWhiteSpace(blobUrl))
            throw new DomainException("BlobUrl cannot be empty");

        var newVersion = new ProjectVersion(Id, CurrentVersionNumber, newHash, blobUrl);
        _versions.Add(newVersion);
        UpdatedAt = DateTime.UtcNow;
    }

    public ProjectVersion GetVersion(int versionNumber)
    {
        var version = _versions.FirstOrDefault(v => v.VersionNumber == versionNumber);
        if (version == null)
            throw new DomainException($"Version {versionNumber} not found");
        return version;
    }

    public ProjectVersion GetLatestVersion()
    {
        return _versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
        
    }

    public void AddChangeLogToVersion(int versionNumber, string changes)
    {
        var version = GetVersion(versionNumber);
        version.SetChangelog(changes);
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetVersionCount() => _versions.Count;

    public string GetDisplayName() => $"{ProjectName} (v{CurrentVersionNumber})";

    


}