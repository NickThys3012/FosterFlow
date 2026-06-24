using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Domain.Interfaces.Repositories;
using FosterFlow.Infrastructure.Persistence;
using FosterFlow.Infrastructure.Persistence.Repositories;
using FosterFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
namespace FosterFlow.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("Default")));
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<ICatRepository, CatRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }
}
