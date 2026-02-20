using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;
using Genshin_Calculator.Services.Interfaces;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class MainViewModel : ObservableRecipient,
    IRecipient<CharacterChangedMessage>,
    IRecipient<RefreshMaterialsRequestMessage>,
    IRecipient<InventoryChangedMessage>, IDropTarget,
    IRecipient<DimmingMessage>
{
    private readonly InventoryService inventoryService;

    private readonly IDialogService dialogService;

    private readonly DataIOService dataIOService;

    [ObservableProperty]
    private bool isDimmed;

    public MainViewModel(InventoryService inventoryService, IDialogService dialogService, DataIOService dataIOService)
    {
        this.inventoryService = inventoryService;
        this.dialogService = dialogService;
        this.dataIOService = dataIOService;

        this.IsActive = true;
        this.RefreshCharacters();
    }

    public ObservableCollection<CharacterCardViewModel> Characters { get; } = [];

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is CharacterCardViewModel && dropInfo.TargetItem is CharacterCardViewModel)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not CharacterCardViewModel sourceItem ||
            dropInfo.TargetItem is not CharacterCardViewModel targetItem)
        {
            return;
        }

        int oldIndex = this.Characters.IndexOf(sourceItem);
        int newIndex = this.Characters.IndexOf(targetItem);

        if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
        {
            return;
        }

        this.Characters.Move(oldIndex, newIndex);

        this.UpdatePriorities();

        this.RefreshAllMaterials();
        this.dataIOService.Save();
    }

    public void Receive(CharacterChangedMessage message)
    {
        var character = message.Value;

        if (character.Deleted)
        {
            var vm = this.Characters.FirstOrDefault(c => c.Character == character);
            if (vm != null)
            {
                this.Characters.Remove(vm);
                this.RefreshAllMaterials();
            }

            return;
        }

        if (this.Characters.All(c => c.Character != character))
        {
            this.RefreshCharacters();
            return;
        }

        this.RefreshAllMaterials();
        this.dataIOService.Save();
    }

    public void Receive(InventoryChangedMessage message)
    {
        this.RefreshAllMaterials();
        this.dataIOService.Save();
    }

    public void Receive(RefreshMaterialsRequestMessage message)
    {
        this.RefreshAllMaterials();
        this.dataIOService.Save();
    }

    public void Receive(DimmingMessage message)
    {
        this.IsDimmed = message.IsEnabled;
    }

    private void UpdatePriorities()
    {
        for (int i = 0; i < this.Characters.Count; i++)
        {
            this.Characters[i].Character.Priority = i;
        }
    }

    private void RefreshCharacters()
    {
        this.Characters.Clear();
        var inventory = this.inventoryService.GetInventory();
        var missingByCharacter = this.inventoryService.CalculateMissingMaterials(inventory);

        var sortedChars = inventory.NotDeletedCharacters.OrderBy(c => c.Priority);

        foreach (var character in sortedChars)
        {
            var materials = missingByCharacter.GetValueOrDefault(character) ?? [];
            var charVm = this.CreateCharacterViewModel(character, materials);
            this.Characters.Add(charVm);
        }
    }

    private CharacterCardViewModel CreateCharacterViewModel(Character character, List<Material> materials)
    {
        return new CharacterCardViewModel(character, materials, this.dialogService, this.inventoryService);
    }

    private void RefreshAllMaterials()
    {
        Inventory inventory = this.inventoryService.GetInventory();
        var missingByCharacter = this.inventoryService.CalculateMissingMaterials(inventory);

        foreach (var charVm in this.Characters)
        {
            charVm.RequiredMaterials = missingByCharacter.GetValueOrDefault(charVm.Character) ?? [];
        }
    }
}