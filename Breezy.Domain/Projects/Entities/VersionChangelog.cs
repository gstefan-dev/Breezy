using Breezy.Domain.Shared;

namespace Breezy.Domain.Projects.Entities;

public class VersionChangelog : Entity
{
    public Guid ProjectVersionId { get; private set; }
    public string Changes { get; private set; }

    private VersionChangelog()
    {
    }

    public VersionChangelog(Guid projectVersionId, string changes) : base(Guid.NewGuid())
    {
        if (projectVersionId == Guid.Empty)
            throw new DomainException("ProjectVersionId cannot be empty");

        ProjectVersionId = projectVersionId;
        Changes = !string.IsNullOrWhiteSpace(changes) ? changes.Trim() : string.Empty;
    }

    public void UpdateChanges(string changes)
    {
        Changes = !string.IsNullOrWhiteSpace(changes) ? changes.Trim() : string.Empty;
    }

}