using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Services.Interfaces;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class ToolsPanelViewModel : ObservableRecipient
{
    private readonly IDialogService dialogService;

    public ToolsPanelViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    [RelayCommand]
    private void ManageInventory()
    {
        this.dialogService.ShowInventory();
    }

    [RelayCommand]
    private void ManagePriority()
    {
        this.dialogService.ShowPriority();
    }

    [RelayCommand]
    private void ShowCharacterSelector()
    {
        this.dialogService.ShowCharacterSelector();
    }
}