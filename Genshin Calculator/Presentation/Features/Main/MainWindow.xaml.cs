using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator.Presentation.Features.Main;

public partial class MainWindow : Window
{
    public MainWindow()
        : this(App.Services.GetRequiredService<MainView>())
    {
    }

    public MainWindow(MainView viewModel)
    {
        this.InitializeComponent();
        this.Content = viewModel;
    }
}