using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Infrastructure;
using Genshin_Calculator.Infrastructure.Repositories;
using Genshin_Calculator.Presentation.Features.Main;
using Genshin_Calculator.Presentation.Features.Tools;
using Genshin_Calculator.Presentation.Services;
using Genshin_Calculator.Services;
using Genshin_Calculator.Services.MaterialProviders;
using Genshin_Calculator.Services.State;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

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

        services.AddSingleton<IStaticDataRepository, WpfStaticDataRepository>();
        services.AddSingleton<IUserDataRepository, LocalFileUserDataRepository>();
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
        services.AddSingleton<ExpMaterialProvider>();

        services.AddSingleton<IMaterialProviderFactory, MaterialProviderFactory>();

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