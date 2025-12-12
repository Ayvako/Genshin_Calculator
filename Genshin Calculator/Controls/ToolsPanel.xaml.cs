using System.ComponentModel;
using System.Windows.Controls;
using Genshin_Calculator.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Genshin_Calculator.Controls;

public partial class ToolsPanelControl : UserControl
{
    public ToolsPanelControl()
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