using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.ValueObjects;

namespace Breezy.Domain.SoundPacks.Entities;

public class SoundPackExport
{
    public Guid SoundPackId { get; private set; }
    public ExportFormatEnum Format { get; private set; }
    public string BlobUrl { get; private set; }
    public DateTime ExportedAt { get; private set; }
    
    private SoundPackExport(){}
    
    public SoundPackExport(Guid soundPackId, ExportFormatEnum format, string blobUrl)
    {
        if (soundPackId == Guid.Empty)
            throw new DomainException("SoundPackId cannot be empty");

        if (string.IsNullOrWhiteSpace(blobUrl))
            throw new DomainException("BlobUrl cannot be empty");

        SoundPackId = soundPackId;
        Format = format;
        BlobUrl = blobUrl;
        ExportedAt = DateTime.UtcNow;
    }
    
    public string GetFileExtension() => Format switch
    {
        ExportFormatEnum.Rar => "rar",
        ExportFormatEnum.Zip => "zip",
        ExportFormatEnum.Tar => "tar",
        _ => "rar"
    }; 
    
    public bool IsOlderThan(int days) => (DateTime.UtcNow - ExportedAt).TotalDays > days;
}