using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Presentation.Features.Dialogs;

public partial class UpgradeCharacterDialogViewModel : ObservableObject
{
    private readonly IDialogService dialogService;

    private readonly IInventoryService inventoryService;

    [ObservableProperty]
    private bool? dialogResult;

    [ObservableProperty]
    private IReadOnlyCollection<MaterialRequirement> materials;

    public UpgradeCharacterDialogViewModel(Character character, IReadOnlyCollection<MaterialRequirement> materialRequirementUIs, IDialogService dialogService, IInventoryService inventoryService)
    {
        this.Character = character;
        this.Materials = materialRequirementUIs;
        this.dialogService = dialogService;
        this.inventoryService = inventoryService;
    }

    public event Action? RequestClose;

    public Character Character { get; set; }

    [RelayCommand]
    private void Save()
    {
        this.DialogResult = true;

        this.RequestClose?.Invoke();
    }

    [RelayCommand]
    private void OpenAddItem(Material material)
    {
        var relatedMaterials = this.inventoryService.GetRelatedMaterials(this.Character, material);
        this.dialogService.ShowAddMaterialsDialog(relatedMaterials);
        this.RefreshMaterials();
    }

    private void RefreshMaterials()
    {
        var inventory = this.inventoryService.GetInventory();
        var missingMap = this.inventoryService.CalculateMissingMaterials(inventory);

        if (missingMap.TryGetValue(this.Character, out var requirements))
        {
            this.Materials = [.. requirements.Where(m => m.TakenFromInventory > 0 || m.CraftedAmount > 0)];
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        this.DialogResult = false;

        this.RequestClose?.Invoke();
    }
}