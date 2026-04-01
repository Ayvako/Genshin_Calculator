using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Presentation.Features.Dialogs;

public partial class UpgradeCharacterDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private bool? dialogResult;

    public UpgradeCharacterDialogViewModel(Character character, IReadOnlyCollection<MaterialRequirement> materialRequirementUIs)
    {
        this.Character = character;
        this.Materials = [.. materialRequirementUIs.Where(m => m.TakenFromInventory > 0 || m.CraftedAmount > 0)];
    }

    public event Action? RequestClose;

    public Character Character { get; set; }

    public IReadOnlyCollection<MaterialRequirement> Materials { get; set; }

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