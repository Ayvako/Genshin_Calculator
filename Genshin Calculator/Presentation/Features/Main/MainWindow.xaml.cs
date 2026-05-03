using System.Windows;

namespace Genshin_Calculator.Presentation.Features.Main;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;
    }
}