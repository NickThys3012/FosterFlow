using FluentValidation;
using FosterFlow.Application.Common.Behaviours;
using FosterFlow.Contracts.Validators.Cats;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace FosterFlow.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Register validators from Contracts assembly (shared with Web)
        services.AddValidatorsFromAssembly(typeof(CreateCatRequestValidator).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>));
    }
}
