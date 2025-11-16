using System.ComponentModel;
using System.Windows.Controls;
using Genshin_Calculator.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator.Views
{
    /// <summary>
    /// Логика взаимодействия для ToolsPanel.xaml.
    /// </summary>
    public partial class ToolsPanel : UserControl
    {
        public ToolsPanel()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                this.InitializeComponent();
                return;
            }

            var vm = App.Services.GetRequiredService<ToolsPanelViewModel>();
            this.Initialize(vm);
        }

        private void Initialize(ToolsPanelViewModel vm)
        {
            this.InitializeComponent();
            this.DataContext = vm;
        }
    }
}