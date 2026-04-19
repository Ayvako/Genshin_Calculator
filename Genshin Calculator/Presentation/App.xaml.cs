using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
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
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace Genshin_Calculator.Presentation;

public partial class App : Application
{
    private bool isInitialized = false;

    public App()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();
    }

    public static IServiceProvider Services { get; private set; } = null!;

    public static IConfiguration Configuration { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var dataIOService = Services.GetRequiredService<IDataIOService>();
            await dataIOService.ImportAsync();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            this.isInitialized = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting the application: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (this.isInitialized)
        {
            var dataService = Services.GetService<IDataIOService>();
            dataService?.Save();
        }

        base.OnExit(e);
    }

    private static T LoadResource<T>(string fileName)
    {
        var resourceName = $"Genshin_Calculator.Resources.Json.{fileName}";
        var assembly = typeof(App).Assembly;

        using Stream? stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);

        return new JsonSerializer().Deserialize<T>(jsonReader)
            ?? throw new InvalidOperationException($"Failed to parse JSON: {fileName}");
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);

        var levelData = LoadResource<LevelData>("LevelCosts.json");
        var skillData = LoadResource<SkillLevelData>("SkillCosts.json");

        services.AddSingleton(levelData);
        services.AddSingleton(skillData);

        services.AddSingleton<IDataRepository, JsonGameDataRepository>();
        services.AddSingleton<IUserDataRepository, LocalFileUserDataRepository>();

        services.AddSingleton<IDataIOService, DataIOService>();
        services.AddTransient<IInventoryService, InventoryService>();
        services.AddTransient<ICharacterService, CharacterService>();
        services.AddTransient<ISkillUpgradeService, SkillUpgradeService>();
        services.AddTransient<ICharacterUpgradeService, CharacterUpgradeService>();
        services.AddTransient<IAlchemyService, AlchemyService>();
        services.AddTransient<IExperienceService, ExperienceService>();

        services.AddSingleton<IInventoryStore, InventoryStore>();
        services.AddSingleton<IViewService, ViewService>();

        services.AddSingleton<GemMaterialProvider>();
        services.AddSingleton<SkillMaterialProvider>();
        services.AddSingleton<EnemyMaterialProvider>();
        services.AddSingleton<ExpMaterialProvider>();

        services.AddSingleton<IMaterialProvider>(sp => sp.GetRequiredService<SkillMaterialProvider>());
        services.AddSingleton<IMaterialProvider>(sp => sp.GetRequiredService<GemMaterialProvider>());
        services.AddSingleton<IMaterialProvider>(sp => sp.GetRequiredService<EnemyMaterialProvider>());
        services.AddSingleton<IMaterialProvider>(sp => sp.GetRequiredService<ExpMaterialProvider>());

        services.AddSingleton<IMaterialProviderFactory, MaterialProviderFactory>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<ToolsPanelViewModel>();
        services.AddTransient<MainWindow>();
        services.AddTransient<MainView>();
    }
}