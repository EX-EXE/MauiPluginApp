using MauiPluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace MauiPluginApp.Services;

public class AvaloniaPluginLoadContext : PluginLoadContext<IPluginBase>
{
    public AvaloniaPluginLoadContext(string pluginPath)
        : base(pluginPath)
    {
    }
}

public class PluginLoadContext<T> : AssemblyLoadContext
    where T : class
{
    public static PluginLoadContext<T> LoadAssembly(ReadOnlySpan<char> path)
    {
        var pluginLocation = Path.GetFullPath(path.ToString().Replace('\\', Path.DirectorySeparatorChar));
        Console.WriteLine($"Loading commands from: {pluginLocation}");
        var loadContext = new PluginLoadContext<T>(pluginLocation);
        return loadContext;
    }

    private static IEnumerable<T> CreatePluginInstance(Assembly assembly)
    {
        int count = 0;

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                var result = Activator.CreateInstance(type) as T;
                if (result != null)
                {
                    count++;
                    yield return result;
                }
            }
        }
        if (count == 0)
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new InvalidOperationException(
                $"Can't find any type which implements {typeof(T).FullName} in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }

    private AssemblyDependencyResolver _resolver;

    private string path = string.Empty;
    public PluginLoadContext(string pluginPath)
        : base(isCollectible: true)
    {
        path = pluginPath;
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    public IEnumerable<T> LoadPlugin()
    {
        var assembly = LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        return CreatePluginInstance(assembly);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            using var fileStream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);
            return LoadFromStream(fileStream);
        }

        return null;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }
        return nint.Zero;
    }
}