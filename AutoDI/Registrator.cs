using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace IoC.AutoDI;

public static class Registrator
{
    public static IServiceCollection AddAutoDiRegistration(
        this IServiceCollection services
    )
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
        {
            return services;
        }
        return services.Register(entryAssembly);
    }

    public static IServiceCollection AddAutoDiRegistration(
        this IServiceCollection services,
        params Type[] assemblyMarkerTypes
    )
    {
        var assemblies = assemblyMarkerTypes
            .Select(assemblyMarkerType => assemblyMarkerType.Assembly)
            .Distinct()
            .ToArray();

        return services.Register(assemblies);
    }

    private static IServiceCollection Register(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        var concreteClasses = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(candidateType => candidateType.IsClass && !candidateType.IsAbstract);

        var interfaceOwners = new Dictionary<Type, Type>();

        foreach (var concreteClass in concreteClasses)
        {
            Type[] serviceInterfaces =
                ServiceInterface.GetServiceInterfaces(concreteClass);

            RegisterClass.Register(
                services,
                concreteClass,
                interfaceOwners,
                serviceInterfaces
            );
        }

        return services;
    }
}