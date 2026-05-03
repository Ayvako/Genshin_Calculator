using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Application.Services;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Features.Characters;
using Genshin_Calculator.Presentation.Services;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Genshin_Calculator.Presentation.Features.Main;

public partial class MainViewModel : ObservableRecipient,
    IRecipient<CharacterChangedMessage>,
    IRecipient<RefreshMaterialsRequestMessage>,
    IRecipient<InventoryChangedMessage>,
    IRecipient<DimmingMessage>,
    IDropTarget
{
    private readonly IInventoryService inventoryService;

    private readonly IViewService dialogService;

    private readonly IDataIOService dataIOService;

    private readonly ICharacterService characterService;

    [ObservableProperty]
    private bool isDimmed;

    public MainViewModel(
        IInventoryService inventoryService,
        IViewService dialogService,
        IDataIOService dataIOService,
        ICharacterService characterService)
    {
        this.inventoryService = inventoryService;
        this.dialogService = dialogService;
        this.dataIOService = dataIOService;
        this.characterService = characterService;

        this.IsActive = true;
        this.RefreshCharacters();
        this.RefreshMaterialsAndSave();
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

        this.RefreshMaterialsAndSave();
    }

    public void Receive(CharacterChangedMessage message)
    {
        var character = message.Value;

        if (character.Deleted)
        {
            if (this.RemoveCharacter(character))
            {
                this.RefreshMaterialsAndSave();
            }

            return;
        }

        if (!this.Characters.Any(c => c.Character == character))
        {
            this.RefreshCharacters();
            return;
        }

        this.RefreshMaterialsAndSave();
    }

    public void Receive(InventoryChangedMessage message) => this.RefreshMaterialsAndSave();

    public void Receive(RefreshMaterialsRequestMessage message) => this.RefreshMaterialsAndSave();

    public void Receive(DimmingMessage message) => this.IsDimmed = message.IsEnabled;

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

        foreach (var character in inventory.NotDeletedCharacters.OrderBy(c => c.Priority))
        {
            var materials = this.GetMaterials(character, missingByCharacter);
            var sortedMaterials = InventoryService.SortMaterialsForDisplay(materials);
            this.Characters.Add(this.CreateCharacterViewModel(character, sortedMaterials));
        }
    }

    private void RefreshMaterialsAndSave()
    {
        this.RefreshAllMaterials();
        this.dataIOService.Save();
    }

    private void RefreshAllMaterials()
    {
        var inventory = this.inventoryService.GetInventory();
        var missingByCharacter = this.inventoryService.CalculateMissingMaterials(inventory);

        foreach (var charVm in this.Characters)
        {
            charVm.RequiredMaterials = InventoryService.SortMaterialsForDisplay(this.GetMaterials(charVm.Character, missingByCharacter));
        }
    }

    private List<MaterialRequirement> GetMaterials(Character character, Dictionary<Character, List<MaterialRequirement>> missingByCharacter)
    {
        if (!character.Activated)
        {
            return [.. this.inventoryService
                .TotalCost(character)
                .Select(m => new MaterialRequirement(m, m.Amount)
                {
                    MissingAmount = m.Amount,
                    TakenFromInventory = 0,
                    CraftedAmount = 0,
                })];
        }

        return missingByCharacter.TryGetValue(character, out var value) ? value : [];
    }

    private CharacterCardViewModel CreateCharacterViewModel(Character character, List<MaterialRequirement> materials)
    {
        return new CharacterCardViewModel(character, materials, this.dialogService, this.inventoryService, this.characterService);
    }

    private bool RemoveCharacter(Character character)
    {
        var vm = this.Characters.FirstOrDefault(c => c.Character == character);
        if (vm is null)
        {
            return false;
        }

        this.Characters.Remove(vm);
        return true;
    }
}