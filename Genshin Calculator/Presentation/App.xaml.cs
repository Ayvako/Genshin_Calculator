using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Infrastructure;
using Genshin_Calculator.Infrastructure.Repositories;
using Genshin_Calculator.Models;
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

    public static IServiceProvider Services { get; private set; } = null!;

    public static IConfiguration Configuration { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        try
        {
            var levelData = LoadResource<LevelData>("Resources/Json/LevelCosts.json");
            services.AddSingleton(levelData);

            var skillData = LoadResource<SkillLevelData>("Resources/Json/SkillCosts.json");
            services.AddSingleton(skillData);

            services.AddSingleton(skillData);
            services.AddSingleton(levelData);

            services.AddSingleton<IStaticDataRepository, WpfStaticDataRepository>();
            services.AddSingleton<IUserDataRepository, LocalFileUserDataRepository>();
            services.AddSingleton<DataIOService>();
            services.AddSingleton<InventoryService>();
            services.AddSingleton<CharacterService>();

            services.AddTransient<ISkillUpgradeService, SkillUpgradeService>();
            services.AddTransient<ICharacterUpgradeService, CharacterUpgradeService>();
            services.AddTransient<ICharacterService, CharacterService>();
            services.AddTransient<IDataIOService, DataIOService>();
            services.AddTransient<IAlchemyService, AlchemyService>();
            services.AddTransient<IExperienceService, ExperienceService>();

            services.AddTransient<IInventoryService, InventoryService>();

            services.AddTransient<ToolsPanelViewModel>();
            services.AddTransient<MainViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<MainView>();

            services.AddSingleton<GemMaterialProvider>();
            services.AddSingleton<SkillMaterialProvider>();
            services.AddSingleton<EnemyMaterialProvider>();
            services.AddSingleton<ExpMaterialProvider>();

            services.AddSingleton<IMaterialProvider, SkillMaterialProvider>();
            services.AddSingleton<IMaterialProvider, GemMaterialProvider>();
            services.AddSingleton<IMaterialProvider, EnemyMaterialProvider>();
            services.AddSingleton<IMaterialProvider, ExpMaterialProvider>();

            services.AddSingleton<IMaterialProviderFactory, MaterialProviderFactory>();

            services.AddSingleton<IDialogService, WpfDialogService>();
            services.AddSingleton<IInventoryStore, InventoryStore>();

            Configuration = builder.Build();

            Services = services.BuildServiceProvider();

            var dataIOService = Services.GetRequiredService<DataIOService>();
            dataIOService.Import();

            var mainWindow = Services.GetRequiredService<MainWindow>();

            mainWindow.Show();
            this.isInitialized = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (this.isInitialized)
        {
            var dataService = Services.GetService<DataIOService>();
            dataService?.Save();
        }

        base.OnExit(e);
    }

    private static T LoadResource<T>(string relativePath)
    {
        var uri = new Uri($"pack://application:,,,/{relativePath}");
        var resourceInfo = Application.GetResourceStream(uri)
            ?? throw new FileNotFoundException($"Resource not found: {relativePath}");

        using var reader = new StreamReader(resourceInfo.Stream);
        var json = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<T>(json)
            ?? throw new InvalidOperationException($"Failed to parse JSON: {relativePath}");
    }
}