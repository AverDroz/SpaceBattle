using System.Collections.Concurrent;

namespace SpaceGame.Core.IoC;

public static class IoC
{
    private static readonly ConcurrentDictionary<string, Func<object[], object>> _dependencies = new();
    private static readonly ThreadLocal<object?> _currentScope = new(() => null);
    
    public static T Resolve<T>(string dependency, params object[] args)
    {
        if (!_dependencies.TryGetValue(dependency, out var resolver))
        {
            throw new InvalidOperationException($"Dependency '{dependency}' is not registered");
        }
        
        var result = resolver(args);
        if (result is not T typedResult)
        {
            throw new InvalidCastException($"Dependency '{dependency}' cannot be cast to {typeof(T).Name}");
        }
        
        return typedResult;
    }
    
    public static void Register(string dependency, Func<object[], object> resolver)
    {
        _dependencies[dependency] = resolver;
    }
    
    public static void Clear()
    {
        _dependencies.Clear();
    }
}