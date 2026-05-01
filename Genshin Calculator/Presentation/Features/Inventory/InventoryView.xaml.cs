using System.Windows;

namespace Genshin_Calculator.Presentation.Features.Inventory;

public partial class InventoryView : Window
{
    public InventoryView()
    {
        this.InitializeComponent();

        this.Loaded += new RoutedEventHandler(this.MainWindowLoaded);
    }

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is InventoryViewModel viewModel)
        {
            _ = viewModel.LoadDataAsync();
        }
    }
}