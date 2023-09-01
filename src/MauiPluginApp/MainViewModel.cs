using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPluginApp.Services;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPluginApp;


public partial class MainViewModel : ObservableObject
{
    public PluginService? PluginService { get; set; } = null;

    [ObservableProperty]
    private PluginInfo? selectedPlugin = null;

    [ObservableProperty]
    private ContentView control = null;

    public MainViewModel(PluginService pluginService)
    {
        PluginService = pluginService;
    }

    [RelayCommand]
    public async Task RunAsync(object info)
    {
        if (info is PluginInfo pluginInfo)
        {
            if (PluginService != null)
            {
                if (!pluginInfo.IsLoaded)
                {
                    await PluginService.LoadPluginAsync(pluginInfo, default).ConfigureAwait(false);
                    SelectedPlugin = pluginInfo;
                }
                else
                {
                    if (pluginInfo == SelectedPlugin)
                    {
                        SelectedPlugin = null;
                    }
                    await PluginService.UnloadPluginAsync(pluginInfo, default).ConfigureAwait(false);
                }
            }
        }
    }
    partial void OnSelectedPluginChanged(PluginInfo? value)
    {
        if (value != null && value.Data != null && value.Data.ControlInfo.MainControl != null)
        {
            Control = value.Data.ControlInfo.MainControl;
        }
        else
        {
            Control = null;
        }
    }
}
