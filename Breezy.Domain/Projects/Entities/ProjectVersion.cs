using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Projects.Entities;

public class ProjectVersion : Entity
{
    public Guid ProjectId { get; private set; }
    public int VersionNumber { get; private set; }
    public ProjectHash Hash { get; private set; }
    public string BlobUrl { get; private set; }
    public DateTime UploadedAt { get; private set; }
    
    private VersionChangelog _changelog;
    public VersionChangelog Changelog => _changelog;

    private ProjectVersion() { }

    public ProjectVersion(
        Guid projectId,
        int versionNumber,
        ProjectHash hash,
        string blobUrl) : base(Guid.NewGuid())
    {
        if (projectId == Guid.Empty)
            throw new DomainException("ProjectId cannot be empty");

        if (versionNumber <= 0)
            throw new DomainException("VersionNumber must be positive");

        if (string.IsNullOrWhiteSpace(blobUrl))
            throw new DomainException("BlobUrl cannot be empty");

        if (hash == null)
            throw new DomainException("Hash cannot be null");

        ProjectId = projectId;
        VersionNumber = versionNumber;
        Hash = hash;
        BlobUrl = blobUrl.Trim();
        UploadedAt = DateTime.UtcNow;
    }
    
    public void SetChangelog(string changes)
    {
        _changelog = new VersionChangelog(Id, changes);
        UpdatedAt = DateTime.UtcNow;
    }

  
    public bool HasSameHash(ProjectHash otherHash)
    {
        return Hash == otherHash;
    }
}