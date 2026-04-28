using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Presentation.Features.Dialogs;

public partial class UpgradeCharacterDialogViewModel : ObservableObject
{
    private readonly IViewService dialogService;

    private readonly IInventoryService inventoryService;

    [ObservableProperty]
    private bool? dialogResult;

    [ObservableProperty]
    private IReadOnlyCollection<MaterialRequirement> materials = [];

    [ObservableProperty]
    private IReadOnlyCollection<Material> materialsToCraft = [];

    public UpgradeCharacterDialogViewModel(
    Character character,
    IViewService dialogService,
    IInventoryService inventoryService)
    {
        this.Character = character;
        this.dialogService = dialogService;
        this.inventoryService = inventoryService;

        this.RefreshMaterials();
    }

    public event Action? RequestClose;

    public Character Character { get; }

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

            this.UpdateMaterialsToCraft();
        }
    }

    private void UpdateMaterialsToCraft()
    {
        this.MaterialsToCraft = [.. this.Materials
            .SelectMany(m => m.AlchemyCosts)
            .GroupBy(a => a.Name)
            .Select(g =>
            {
                var first = g.First();

                return new Material(g.Key, first.Type, first.Rarity, g.Sum(x => x.Amount));
            })];
    }
}