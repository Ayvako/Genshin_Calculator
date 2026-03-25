using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Presentation.Features.Dialogs;

public partial class UpdateCharacterDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private bool? dialogResult;

    public UpdateCharacterDialogViewModel(IReadOnlyCollection<MaterialRequirementUI> materialRequirementUIs)
    {
        this.Materials = [.. materialRequirementUIs.Where(m => m.TakenFromInventory > 0 || m.CraftedAmount > 0)];
    }

    public event Action? RequestClose;

    public IReadOnlyCollection<MaterialRequirementUI> Materials { get; set; }

    [RelayCommand]
    private void Save()
    {
        this.DialogResult = true;

        this.RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        this.DialogResult = false;

        this.RequestClose?.Invoke();
    }
}