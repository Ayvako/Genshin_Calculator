using System;
using System.Windows;
using Genshin_Calculator.Presentation.ViewModels;
using Genshin_Calculator.Presentation.Views;
using Genshin_Calculator.Services;
using Genshin_Calculator.Services.Interfaces;
using Genshin_Calculator.Services.Materials;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator.Presentation;

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

        services.AddTransient<ISkillUpgradeService, SkillUpgradeService>();
        services.AddTransient<ICharacterUpgradeService, CharacterUpgradeService>();

        services.AddTransient<ToolsPanelViewModel>();
        services.AddTransient<MainViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();

        services.AddSingleton<GemMaterialProvider>();
        services.AddSingleton<SkillMaterialProvider>();
        services.AddSingleton<EnemyMaterialProvider>();

        services.AddSingleton<IDialogService, WpfDialogService>();
        services.AddSingleton<IInventoryStore, InventoryStore>();

        Configuration = builder.Build();

        Services = services.BuildServiceProvider();

        var dataIOService = Services.GetRequiredService<DataIOService>();
        dataIOService.Import();

        var mainWindow = Services.GetRequiredService<MainWindow>();

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var dataService = Services.GetService<DataIOService>();
        dataService?.Save();

        base.OnExit(e);
    }
}