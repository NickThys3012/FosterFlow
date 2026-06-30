using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FosterFlow.Application.Common.Interfaces;

namespace FosterFlow.Infrastructure.Services;

public sealed class BlobFileStorageService : IFileStorageService
{
    private const string ContainerName = "cat-photos";
    private readonly BlobContainerClient _containerClient;

    public BlobFileStorageService(BlobServiceClient blobServiceClient)
    {
        _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string sourceFileName, string contentType, string folder, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var extension = Path.GetExtension(sourceFileName);
        var blobFileName = $"{Guid.NewGuid():N}{extension}";
        var blobName = string.IsNullOrWhiteSpace(folder)
            ? blobFileName
            : $"{folder.Trim('/').Trim('\\')}/{blobFileName}";

        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            },
            cancellationToken: ct);

        return blobClient.Uri.ToString();
    }
}
