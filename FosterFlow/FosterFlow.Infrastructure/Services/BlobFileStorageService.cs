using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FosterFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
namespace FosterFlow.Infrastructure.Services;

public sealed class BlobFileStorageService : IFileStorageService
{
    private const string ContainerName = "cat-photos";
    private readonly BlobContainerClient _containerClient;
    private readonly Uri? _publicBlobBaseUri;

    public BlobFileStorageService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> options)
    {
        _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var configuredPublicBaseUrl = options.Value.PublicBaseUrl;
        _publicBlobBaseUri = string.IsNullOrWhiteSpace(configuredPublicBaseUrl)
            ? null
            : Uri.TryCreate(configuredPublicBaseUrl, UriKind.Absolute, out var parsedUri)
                ? parsedUri
                : throw new InvalidOperationException("BlobStorage:PublicBaseUrl must be a valid absolute URI.");
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        await _containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var extension = Path.GetExtension(fileName);
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
            ct);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "Blob storage credentials must support SAS generation to return image URLs.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name, BlobName = blobClient.Name, Resource = "b", ExpiresOn = DateTimeOffset.UtcNow.AddDays(7)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        if (_publicBlobBaseUri is null)
        {
            return sasUri.ToString();
        }

        var publicUriBuilder = new UriBuilder(sasUri)
        {
            Scheme = _publicBlobBaseUri.Scheme, Host = _publicBlobBaseUri.Host, Port = _publicBlobBaseUri.IsDefaultPort ? -1 : _publicBlobBaseUri.Port
        };

        return publicUriBuilder.Uri.ToString();
    }
}
