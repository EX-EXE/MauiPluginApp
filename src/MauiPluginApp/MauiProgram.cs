using MauiPluginApp.Services;
using Microsoft.Extensions.Logging;

namespace MauiPluginApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif


            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>(x => new MainPage() { BindingContext = x.GetRequiredService<MainViewModel>() });

            builder.Services.AddSingleton<PluginService>();
            builder.Services.AddHostedService<PluginService>(x => x.GetRequiredService<PluginService>());

            return builder.Build();
        }
    }
}
