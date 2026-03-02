using FluentValidation;
using GymWebApp.Application.CQRS;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GymWebApp.Application.Extensions;

public static class ApplicationLayerRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ITrainerService, TrainerService>();

        return services;
    }
}