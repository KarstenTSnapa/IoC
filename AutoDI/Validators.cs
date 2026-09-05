namespace IoC.AutoDI;
internal static class Validators
{
    internal static void ValidateLifetimeMatch(
        Type concreteClass,
        Type serviceInterface,
        (bool Scoped, bool Singleton, bool Transient) lifetime)
    {
        bool mismatch =
            (typeof(IScopedInterface).IsAssignableFrom(serviceInterface) && !lifetime.Scoped) ||
            (typeof(ISingletonInterface).IsAssignableFrom(serviceInterface) && !lifetime.Singleton) ||
            (typeof(ITransientInterface).IsAssignableFrom(serviceInterface) && !lifetime.Transient);

        if (mismatch)
        {
            throw new InvalidOperationException(
                $"'{concreteClass.Name}' implements '{serviceInterface.Name}' but its lifetime " +
                $"(Scoped: {lifetime.Scoped}, Transient: {lifetime.Transient}, Singleton: {lifetime.Singleton}) " +
                $"doesn't match the lifetime required by '{serviceInterface.Name}'.");
        }
    }
    internal static void ValidateNoDuplicates(
        Type concreteClass,
        Type serviceInterface,
        Dictionary<Type, Type> interfaceOwners)
    {
        bool allowsMultiple = serviceInterface
            .GetCustomAttributes(typeof(AllowMultipleAttribute), false)
            .Length != 0;

        if (allowsMultiple)
        {
            return;
        }

        if (interfaceOwners.TryGetValue(serviceInterface, out var existingOwner))
        {
            throw new InvalidOperationException(
                $"Both '{existingOwner.Name}' and '{concreteClass.Name}' implement " +
                $"'{serviceInterface.Name}'. If this is intentional, mark " +
                $"'{serviceInterface.Name}' with [AllowMultiple].");
        }

        interfaceOwners[serviceInterface] = concreteClass;
    }
}