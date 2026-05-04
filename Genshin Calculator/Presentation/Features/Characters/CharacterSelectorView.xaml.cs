using System.Windows;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterSelectorView : Window
{
    public CharacterSelectorView()
    {
        this.InitializeComponent();
        this.Loaded += new RoutedEventHandler(this.MainWindowLoaded);
    }

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is CharacterSelectorViewModel viewModel)
        {
            _ = viewModel.InitializeAsync();
        }
    }
}