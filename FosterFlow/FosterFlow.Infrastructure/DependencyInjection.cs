using Azure.Storage.Blobs;
using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Interfaces.Repositories;
using FosterFlow.Infrastructure.Persistence;
using FosterFlow.Infrastructure.Persistence.Repositories;
using FosterFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace FosterFlow.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("Database")));
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<ICatRepository, CatRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IIdentityService, IdentityService>();

        var blobConnectionString = config.GetConnectionString("BlobStorage");
        if (string.IsNullOrWhiteSpace(blobConnectionString))
        {
            throw new InvalidOperationException("Could not find a connection string named 'BlobStorage'.");
        }

        services.AddSingleton(new BlobServiceClient(blobConnectionString));
        services.AddScoped<IFileStorageService, BlobFileStorageService>();
    }
}
