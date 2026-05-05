using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Features.Inventory;

public partial class InventoryViewModel : ObservableObject
{
    private readonly IInventoryService inventoryService;

    private List<Material> originalMaterials = [];

    [ObservableProperty]
    private InventoryFilterOption? selectedFilter;

    public InventoryViewModel(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;

        this.MaterialsView = CollectionViewSource.GetDefaultView(this.Materials);
        this.MaterialsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MaterialViewModel.Type)));
        this.MaterialsView.Filter = this.FilterMaterials;

        this.FilterOptions =
        [
            new() { Value = null },
            .. Enum.GetValues<MaterialTypes>().Select(x => new InventoryFilterOption { Value = x }),
        ];

        this.SelectedFilter = this.FilterOptions[0];
    }

    public event EventHandler<bool>? CloseRequested;

    public List<InventoryFilterOption> FilterOptions { get; }

    public BulkObservableCollection<MaterialViewModel> Materials { get; } = [];

    public ICollectionView MaterialsView { get; }

    public async Task LoadDataAsync()
    {
        var inventory = this.inventoryService.GetInventory();
        this.originalMaterials = inventory.Materials;

        var viewModels = await Task.Run(() =>
            inventory.Materials.Select(m => new MaterialViewModel(m.Clone())).ToList());

        this.Materials.AddRange(viewModels);
    }

    partial void OnSelectedFilterChanged(InventoryFilterOption? value)
    {
        MaterialsView.Refresh();
    }

    private bool FilterMaterials(object obj)
    {
        var filterValue = this.SelectedFilter?.Value;

        if (filterValue == null)
        {
            return true;
        }

        if (obj is MaterialViewModel material)
        {
            return material.Type == filterValue;
        }

        return false;
    }

    [RelayCommand]
    private void Cancel()
    {
        this.CloseRequested?.Invoke(this, false);
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var cloned in this.Materials)
        {
            var original = this.originalMaterials.FirstOrDefault(m => m.Name == cloned.Name);
            if (original != null)
            {
                original.Amount = cloned.Amount;
            }
        }

        WeakReferenceMessenger.Default.Send(new InventoryChangedMessage());
        this.CloseRequested?.Invoke(this, true);
    }
}