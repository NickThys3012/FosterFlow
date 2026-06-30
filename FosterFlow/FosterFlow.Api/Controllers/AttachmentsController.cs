using FluentValidation.Results;
using FosterFlow.Application.Common.Exceptions;
using FosterFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FosterFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Shelter")]
public class AttachmentsController : ControllerBase
{
    private const long MaxPhotoBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };
    private readonly IFileStorageService _fileStorage;

    public AttachmentsController(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost("photo")]
    [RequestSizeLimit(MaxPhotoBytes)]
    public async Task<IActionResult> UploadPhoto([FromForm] IFormFile? file, CancellationToken ct)
    {
        var uploadFile = ValidatePhoto(file);
        await using var stream = uploadFile.OpenReadStream();
        var url = await _fileStorage.SaveFileAsync(stream, uploadFile.FileName, uploadFile.ContentType, "cats", ct);

        return Ok(new
        {
            url
        });
    }

    private static IFormFile ValidatePhoto(IFormFile? file)
    {
        var failures = new List<ValidationFailure>();

        if (file is null || file.Length <= 0)
        {
            failures.Add(new ValidationFailure("file", "A photo is required."));
        }
        else
        {
            if (file.Length > MaxPhotoBytes)
            {
                failures.Add(new ValidationFailure("file", "Photo must be 10MB or smaller."));
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                failures.Add(new ValidationFailure("file", "Only JPG, PNG, or WebP photos are allowed."));
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                failures.Add(new ValidationFailure("file", "Only JPG, PNG, or WebP photos are allowed."));
            }
        }

        return failures.Count > 0 ? throw new ValidationException(failures) : file!;
    }
}
