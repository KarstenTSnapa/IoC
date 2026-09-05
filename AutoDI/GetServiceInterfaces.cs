namespace IoC.AutoDI;

internal static class ServiceInterface
{
    internal static Type[] GetServiceInterfaces(Type concreteClass)
    {
        return concreteClass.GetInterfaces()
            .Where(interfaceType =>
                typeof(IScopedInterface)
                    .IsAssignableFrom(interfaceType) 
                ||
                typeof(ITransientInterface)
                    .IsAssignableFrom(interfaceType) 
                ||
                typeof(ISingletonInterface)
                    .IsAssignableFrom(interfaceType)
            ).ToArray();
    }
}