using FosterFlow.Api.Controllers;
using FosterFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
namespace FosterFlow.Api.Tests.Controllers;

[TestFixture]
public class AttachmentsControllerTests
{
    [Test]
    public async Task UploadPhoto_WithValidImage_SavesToBlobStorageAndReturnsUrl()
    {
        var storage = Substitute.For<IFileStorageService>();
        storage.SaveFileAsync(Arg.Any<Stream>(), "photo.png", "image/png", "cats", Arg.Any<CancellationToken>())
            .Returns("https://storage.example/cat-photos/cats/photo.png");

        var controller = new AttachmentsController(storage);
        var file = CreatePhotoFile("photo.png", "image/png");

        var result = await controller.UploadPhoto(file, CancellationToken.None) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Value, Is.Not.Null);
            Assert.That(result.Value.GetType().GetProperty("url")?.GetValue(result.Value), Is.EqualTo("https://storage.example/cat-photos/cats/photo.png"));
        }
        await storage.Received(1).SaveFileAsync(Arg.Any<Stream>(), "photo.png", "image/png", "cats", Arg.Any<CancellationToken>());
    }

    private static FormFile CreatePhotoFile(string fileName, string contentType)
    {
        var stream = new MemoryStream([
            1, 2, 3, 4
        ]);
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };
    }
}
