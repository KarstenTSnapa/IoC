using Microsoft.Extensions.DependencyInjection;

namespace IoC.AutoDI;

internal static class RegisterClass
{
    internal static void Register(
        IServiceCollection services,
        Type concreteClass,
        Dictionary<Type, Type> interfaceOwners,
        Type[] serviceInterfaces
    )
    {
        bool classIsScoped = typeof(IScoped).IsAssignableFrom(concreteClass);
        bool classIsSingleton = typeof(ISingleton).IsAssignableFrom(concreteClass);
        bool classIsTransient = typeof(ITransient).IsAssignableFrom(concreteClass);

        (bool Scoped, bool Singleton, bool Transient) lifetime = (
            Scoped: classIsScoped, 
            Singleton: classIsSingleton, 
            Transient: classIsTransient
        );

        foreach (var serviceInterface in serviceInterfaces)
        {
            Validators.ValidateLifetimeMatch(concreteClass, serviceInterface, lifetime);
            Validators.ValidateNoDuplicates(concreteClass, serviceInterface, interfaceOwners);
        }

        if (classIsScoped)
        {
            services.AddScoped(concreteClass);
            foreach (var serviceInterface in serviceInterfaces)
                services.AddScoped(serviceInterface, concreteClass);
        }
        else if (classIsSingleton)
        {
            services.AddSingleton(concreteClass);
            foreach (var serviceInterface in serviceInterfaces)
                services.AddSingleton(serviceInterface, concreteClass);
        }
        else if (classIsTransient)
        {
            services.AddTransient(concreteClass);
            foreach (var serviceInterface in serviceInterfaces)
                services.AddTransient(serviceInterface, concreteClass);
        }
    }
}