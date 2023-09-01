using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiPluginBase;

public interface IPluginBase
{
    ValueTask InitializeAsync(CancellationToken cancellationToken);
    ValueTask FinalizeAsync(CancellationToken cancellationToken);

    PluginControlInfo ControlInfo { get; }
}

public partial class PluginBase : ObservableObject, IPluginBase
{
    [ObservableProperty]
    public PluginControlInfo? _controlInfo;

    public virtual ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
    public virtual ValueTask FinalizeAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}

public partial class PluginControlInfo : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private ContentView? _mainControl = null;

    [ObservableProperty]
    private ContentView? _settingControl = null;
}