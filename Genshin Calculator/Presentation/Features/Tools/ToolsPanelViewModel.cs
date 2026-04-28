using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Presentation.Services;

namespace Genshin_Calculator.Presentation.Features.Tools;

public partial class ToolsPanelViewModel : ObservableRecipient
{
    private readonly IViewService dialogService;

    public ToolsPanelViewModel(IViewService dialogService)
    {
        this.dialogService = dialogService;
    }

    [RelayCommand]
    private void ManageInventory()
    {
        this.dialogService.ShowInventory();
    }

    [RelayCommand]
    private void ShowCharacterSelector()
    {
        this.dialogService.ShowCharacterSelector();
    }
}