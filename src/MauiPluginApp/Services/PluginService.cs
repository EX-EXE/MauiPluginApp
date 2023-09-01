using CommunityToolkit.Mvvm.ComponentModel;
using MauiPluginBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPluginApp.Services;

public partial class PluginInfo : ObservableObject, IDisposable
{
    private static readonly DateTimeOffset InvalidDateTime = DateTimeOffset.MinValue;
    private PluginLoadContext<IPluginBase>? loadContext = null;
    private bool disposedValue;

    [ObservableProperty]
    public string _name = string.Empty;
    [ObservableProperty]
    public string _dll = string.Empty;
    [ObservableProperty]
    public bool _isLoaded = false;

    public IPluginBase? Data { get; set; } = null;

    public DateTimeOffset LoadDateTime { get; set; } = InvalidDateTime;

    public PluginInfo(ReadOnlySpan<char> name, ReadOnlySpan<char> dll)
    {
        Name = name.ToString();
        Dll = dll.ToString();
        IsLoaded = false;
    }

    public async ValueTask LoadAsync(CancellationToken cancellationToken)
    {
        if (loadContext != null)
        {
            throw new InvalidOperationException($"Already Load Plugin : {Name}({Dll})[{LoadDateTime}]");
        }

        loadContext = AvaloniaPluginLoadContext.LoadAssembly(Dll);
        try
        {
            var plugins = loadContext.LoadPlugin().ToArray();
            if (plugins.Length <= 0)
            {
                throw new InvalidOperationException($"No Plugin Class. : {Dll}");
            }
            if (2 <= plugins.Length)
            {
                throw new InvalidOperationException($"No Multi Plugin Class. : {Dll}");
            }
            Data = plugins.First();
            await Data.InitializeAsync(cancellationToken).ConfigureAwait(false);

            IsLoaded = true;
            LoadDateTime = DateTimeOffset.Now;
        }
        catch
        {
            await UnloadAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }

    }

    public async ValueTask UnloadAsync(CancellationToken cancellationToken)
    {
        if (loadContext == null)
        {
            throw new InvalidOperationException($"Not Load Plugin : {Name}({Dll})[{LoadDateTime}]");
        }

        try
        {
            if (Data != null)
            {
                await Data.FinalizeAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error FinalizeAsync Plugin. : {Name}({Dll})\n{ex}");
        }
        Data = null;

        var weekRef = new WeakReference(loadContext, true);
        loadContext.Unload();
        loadContext = null;

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        while (weekRef.IsAlive)
        {
            if (TimeSpan.FromSeconds(0.5) < stopwatch.Elapsed)
            {
                Console.Error.WriteLine($"Error UnloadTimeout Plugin. : {Name}({Dll})");
                break;
            }
            GC.Collect();
            await Task.Delay(100).ConfigureAwait(false);
        }
        IsLoaded = false;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                UnloadAsync(default).GetAwaiter().GetResult();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class PluginService : ObservableBackgroundService
{
    public ObservableCollection<PluginInfo> Plugins { get; } = new();

    public PluginService()
    {
        // ToDo
        Plugins.Add(new PluginInfo("Test", @"T:\Git\MauiPluginApp\src\MauiPluginSample\bin\Debug\net8.0\MauiPluginSample.dll"));
        Plugins.Add(new PluginInfo("Test2", @"T:\Git\MauiPluginApp\src\MauiPluginSample\bin\Debug\net8.0\MauiPluginSample.dll"));
    }

    public ValueTask LoadPluginAsync(PluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        return pluginInfo.LoadAsync(cancellationToken);
    }
    public ValueTask UnloadPluginAsync(PluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        return pluginInfo.UnloadAsync(cancellationToken);
    }

    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // ToDo
            await Task.Delay(TimeSpan.FromSeconds(1.0)).ConfigureAwait(false);
        }
    }
}