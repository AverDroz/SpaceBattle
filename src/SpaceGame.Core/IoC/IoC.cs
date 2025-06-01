using System.Collections.Concurrent;

namespace SpaceGame.Core.IoC;

public static class IoC
{
    private static readonly ConcurrentDictionary<string, Func<object[], object>> _dependencies = new();
    private static readonly ThreadLocal<object?> _currentScope = new(() => null);
    
    public static T Resolve<T>(string dependency, params object[] args)
    {
        // Special handling for built-in IoC.Register command
        if (dependency == "IoC.Register")
        {
            return (T)(object)new RegisterCommand(args);
        }

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

/// <summary>
/// Command for registering dependencies in IoC
/// </summary>
public class RegisterCommand : ICommand
{
    private readonly string _name;
    private readonly Func<object[], object> _factory;

    public RegisterCommand(object[] args)
    {
        if (args.Length != 2)
            throw new ArgumentException("IoC.Register requires exactly 2 arguments: name and factory");

        _name = args[0] as string ?? throw new ArgumentException("First argument must be dependency name");
        _factory = args[1] as Func<object[], object> ?? throw new ArgumentException("Second argument must be factory function");
    }

    public void Execute()
    {
        IoC.Register(_name, _factory);
    }
}