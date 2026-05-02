namespace Breezy.Application.Interfaces;

public interface IAzureBlobService
{
    Task UploadAsync(string fileName, Stream content, string fileType);
    Task<Stream?> DownloadAsync(string fileName);
    Task DeleteAsync(string fileName);
    Task<string?> GetUrlAsync(string fileName);
}