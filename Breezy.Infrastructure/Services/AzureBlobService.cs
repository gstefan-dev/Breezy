using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Breezy.Application.Interfaces;

namespace Breezy.Infrastructure.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "breezy-files";

    public AzureBlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadAsync(string fileName, Stream content, string fileType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(content, new BlobHttpHeaders()
        {
            ContentType = fileType
        });
    }

    public async Task<Stream?> DownloadAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DownloadStreamingAsync();

        return response.Value.Content;
    }

    public async Task DeleteAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync();
    }

    public Task<string?> GetUrlAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        
        var blobClient = containerClient.GetBlobClient(fileName);

        return Task.FromResult<string?>(blobClient.Uri.ToString());
    }
}