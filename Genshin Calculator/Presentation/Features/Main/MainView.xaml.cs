using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator.Presentation.Features.Main
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