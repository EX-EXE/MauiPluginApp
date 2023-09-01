using MauiPluginBase;

namespace MauiPluginSample;

public class SamplePlugin : PluginBase
{

    public override ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        ControlInfo = new PluginControlInfo()
        {
            Name = "Name",
            MainControl = new MainView()
            {
                BindingContext = new MainViewModel(),
            }
        };

        return ValueTask.CompletedTask;
    }
    public override ValueTask FinalizeAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

}