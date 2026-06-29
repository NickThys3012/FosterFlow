using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FosterFlow.Web.Services;

namespace FosterFlow.Web.Components;

public partial class FosterFlowImageUpload : ComponentBase
{
    private const long DefaultMaxAllowedSize = 10 * 1024 * 1024;
    private readonly ImageUploadService _uploadService;
    private bool _isUploading;
    private string? _uploadError;

    public FosterFlowImageUpload(ImageUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [Parameter] public string? PhotoUrl { get; set; }
    [Parameter] public EventCallback<string?> PhotoUrlChanged { get; set; }
    [Parameter] public string Title { get; set; } = "Tap to upload a photo of the cat";
    [Parameter] public string Hint { get; set; } = "JPG, PNG, or WebP up to 10MB";
    [Parameter] public string Accept { get; set; } = "image/png,image/jpeg,image/webp";
    [Parameter] public long MaxAllowedSize { get; set; } = DefaultMaxAllowedSize;

    private async Task HandleSelected(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file is null)
        {
            return;
        }

        _uploadError = null;
        _isUploading = true;
        StateHasChanged();

        var (success, error, url) = await _uploadService.UploadAsync(file, MaxAllowedSize);

        _isUploading = false;
        if (!success || string.IsNullOrWhiteSpace(url))
        {
            _uploadError = error ?? "Unable to upload the photo right now.";
            PhotoUrl = null;
            await PhotoUrlChanged.InvokeAsync(null);
            return;
        }

        PhotoUrl = url;
        await PhotoUrlChanged.InvokeAsync(url);
    }
}
