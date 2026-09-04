using System.Reflection;
using AutoDI;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDi;

public static class Registrator
{
    public static IServiceCollection AddAutoDiRegistration(
        this IServiceCollection services
    )
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly is null)
        {
            return services;
        }
        return services.AddAutoDiRegistration(assembly);
    }

    public static IServiceCollection AddAutoDiRegistration(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        var types = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract);

        foreach (var type in types)
        {
            if (typeof(IScoped).IsAssignableFrom(type))
                services.AddScoped(type);
            else if (typeof(ISingleton).IsAssignableFrom(type))
                services.AddSingleton(type);
            else if (typeof(ITransient).IsAssignableFrom(type))
                services.AddTransient(type);
        }

        return services;
    }
}