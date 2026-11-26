using Microsoft.Extensions.Logging;
using ParkinApp.Views;
using ParkinApp.ViewModels;
using ParkinApp.Services;

namespace ParkinApp;

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

        
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginView>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddTransient<MyVehiclesViewModel>();
        builder.Services.AddTransient<MyVehiclesView>();
        builder.Services.AddTransient<AddEditVehicleViewModel>();
        builder.Services.AddTransient<AddEditVehicleView>();
        builder.Services.AddSingleton<ParkinWebAPIProxy>();
        builder.Services.AddTransient<FeedViewModel>();
        builder.Services.AddTransient<FeedView>();
        return builder.Build();
    }
}