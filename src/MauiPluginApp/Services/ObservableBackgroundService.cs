using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPluginApp.Services;


public abstract class ObservableBackgroundService : ObservableObject, IHostedService, IDisposable
{
    private Task? runTask = null;
    private CancellationTokenSource? cancellationTokenSource = null;

    protected abstract Task RunAsync(CancellationToken cancellationToken);

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        runTask = RunAsync(cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (runTask == null)
        {
            return;
        }

        try
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }
        finally
        {
            await runTask.ConfigureAwait(false);
        }

    }

    public virtual void Dispose()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
    }
}