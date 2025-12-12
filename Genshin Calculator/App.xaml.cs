using System;
using System.Windows;
using Genshin_Calculator.Services;
using Genshin_Calculator.ViewModels;
using Genshin_Calculator.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public static IConfiguration Configuration { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        services.AddSingleton<DataIOService>();
        services.AddSingleton<InventoryService>();
        services.AddSingleton<CharacterService>();

        services.AddTransient<ToolsPanelViewModel>();
        services.AddTransient<MainViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();

        Configuration = builder.Build();

        Services = services.BuildServiceProvider();

        var dataIOService = Services.GetRequiredService<DataIOService>();
        dataIOService.Import();

        var mainWindow = Services.GetRequiredService<MainWindow>();

        mainWindow.Show();
    }
}