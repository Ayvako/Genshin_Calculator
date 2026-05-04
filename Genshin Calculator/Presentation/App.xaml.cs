using Genshin_Calculator.Application.Services;
using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Application.State;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Infrastructure;
using Genshin_Calculator.Infrastructure.Repositories;
using Genshin_Calculator.Presentation.Features.Main;
using Genshin_Calculator.Presentation.Features.Splash;
using Genshin_Calculator.Presentation.Features.Tools;
using Genshin_Calculator.Presentation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Genshin_Calculator.Presentation;

public partial class App : System.Windows.Application
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
        var splash = new SplashWindow();
        splash.Show();

        try
        {
            var progress = new SplashProgress(splash);

            var dataIOService = Services.GetRequiredService<IDataIOService>();
            await dataIOService.ImportAsync(progress);
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            this.isInitialized = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting the application: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Shutdown();
        }
        finally
        {
            splash.Close();
        }
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        base.OnSessionEnding(e);
        this.HandleApplicationExit();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        this.HandleApplicationExit();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(Configuration);

        services.AddSingleton<IDataRepository, JsonGameDataRepository>();
        services.AddSingleton<IUserDataRepository, LocalFileUserDataRepository>();
        services.AddSingleton<IEmbeddedDataRepository, EmbeddedResourceRepository>();
        services.AddSingleton<IDataUpdateService, DataUpdateService>();
        services.AddSingleton<IDataIOService, DataIOService>();
        services.AddTransient<IInventoryService, InventoryService>();
        services.AddTransient<ICharacterService, CharacterService>();
        services.AddTransient<ISkillUpgradeService, SkillUpgradeService>();
        services.AddTransient<ICharacterUpgradeService, CharacterUpgradeService>();
        services.AddTransient<IAlchemyService, AlchemyService>();
        services.AddTransient<IExperienceService, ExperienceService>();
        services.AddSingleton<ITalentLevelRules, DefaultTalentLevelRules>();
        services.AddSingleton<InventoryStore>();
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

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<ToolsPanelViewModel>();
    }

    private void HandleApplicationExit()
    {
        if (this.isInitialized)
        {
            var dataService = Services.GetService<IDataIOService>();
            if (dataService != null)
            {
                Task.Run(() => dataService.SaveAsync()).GetAwaiter().GetResult();
            }
        }
    }
}