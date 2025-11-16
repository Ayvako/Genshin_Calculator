using System.Windows;
using Genshin_Calculator.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator;

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