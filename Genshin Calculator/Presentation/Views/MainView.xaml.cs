using System.Windows.Controls;
using Genshin_Calculator.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator.Presentation.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        : this(App.Services.GetRequiredService<MainViewModel>())
        {
        }

        public MainView(MainViewModel vm)
        {
            this.InitializeComponent();
            this.DataContext = vm;
        }
    }
}