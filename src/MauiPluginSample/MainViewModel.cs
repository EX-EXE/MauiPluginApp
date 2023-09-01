using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPluginSample;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public DateTimeOffset createDate = DateTimeOffset.Now;
}
