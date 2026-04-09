using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Features.Inventory;

public partial class InventoryViewModel : ObservableObject
{
    private readonly List<Material> originalMaterials;

    private readonly IInventoryService inventoryService;

    public InventoryViewModel(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
        var inventory = this.inventoryService.GetInventory();

        var clonedMaterials = inventory.Materials.Select(m => m.Clone()).ToList();
        this.originalMaterials = inventory.Materials;
        this.Materials = new ObservableCollection<Material>(clonedMaterials);
        this.MaterialsView = CollectionViewSource.GetDefaultView(this.Materials);
        this.MaterialsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Material.Type)));
    }

    public event EventHandler<bool>? CloseRequested;

    public ObservableCollection<Material> Materials { get; }

    public ICollectionView MaterialsView { get; }

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