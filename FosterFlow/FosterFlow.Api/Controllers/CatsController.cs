using System.IO;
using FosterFlow.Application.Features.Cats.Commands.CreateCat;
using FosterFlow.Application.Features.Cats.Queries.GetCats;
using FosterFlow.Contracts.DTOs.Cats;
using FosterFlow.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
namespace FosterFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatsController : ControllerBase
{
    private const long MaxPhotoBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly ISender _mediator;
    private readonly IWebHostEnvironment _environment;

    public CatsController(ISender mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetCatQuery(id), ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCatRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateCatCommand(request), ct);
        return CreatedAtAction(nameof(Get), new
        {
            id
        }, new
        {
            id
        });
    }

    [HttpPost("photo")]
    [RequestSizeLimit(MaxPhotoBytes)]
    public async Task<IActionResult> UploadPhoto([FromForm] IFormFile? file, CancellationToken ct)
    {
        var uploadFile = ValidatePhoto(file);

        var uploadsRoot = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
            "uploads", "cats");
        Directory.CreateDirectory(uploadsRoot);

        var extension = GetExtension(uploadFile.ContentType);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await uploadFile.CopyToAsync(stream, ct);
        }

        return Ok(new
        {
            url = $"/uploads/cats/{fileName}"
        });
    }

    private static IFormFile ValidatePhoto(IFormFile? file)
    {
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        if (file is null || file.Length <= 0)
        {
            failures.Add(new FluentValidation.Results.ValidationFailure("file", "A photo is required."));
        }
        else
        {
            if (file.Length > MaxPhotoBytes)
            {
                failures.Add(new FluentValidation.Results.ValidationFailure("file", "Photo must be 10MB or smaller."));
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                failures.Add(new FluentValidation.Results.ValidationFailure("file", "Only JPG, PNG, or WebP photos are allowed."));
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                failures.Add(new FluentValidation.Results.ValidationFailure("file", "Only JPG, PNG, or WebP photos are allowed."));
            }
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return file!;
    }

    private static string GetExtension(string? contentType) => contentType?.ToLowerInvariant() switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/webp" => ".webp",
        _ => throw new ValidationException([
            new FluentValidation.Results.ValidationFailure("file", "Only JPG, PNG, or WebP photos are allowed.")
        ])
    };
}
