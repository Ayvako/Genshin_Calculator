using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Application.Services;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Presentation.Features.Characters;
using Genshin_Calculator.Presentation.Services;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Genshin_Calculator.Presentation.Features.Main;

public partial class MainViewModel : ObservableRecipient,
    IRecipient<CharacterChangedMessage>,
    IRecipient<RefreshMaterialsRequestMessage>,
    IRecipient<InventoryChangedMessage>,
    IRecipient<DimmingMessage>,
    IDropTarget
{
    private readonly SemaphoreSlim syncLock = new(1, 1);

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

        System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this.Characters, new object());
        this.IsActive = true;

        _ = this.InitializeAsync();
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

    public async void Drop(IDropInfo dropInfo)
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

        await this.RefreshMaterialsAndSaveAsync();
    }

    public async void Receive(CharacterChangedMessage message)
    {
        try
        {
            var character = message.Value;

            if (character.Deleted)
            {
                if (this.RemoveCharacter(character))
                {
                    await this.RefreshMaterialsAndSaveAsync();
                }

                return;
            }

            if (!this.Characters.Any(c => c.Character.Model == character))
            {
                await this.RefreshCharactersAsync();
                return;
            }

            await this.RefreshMaterialsAndSaveAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error processing CharacterChangedMessage: {ex.Message}");
        }
    }

    public async void Receive(InventoryChangedMessage message) => await this.RefreshMaterialsAndSaveAsync();

    public async void Receive(RefreshMaterialsRequestMessage message) => await this.RefreshMaterialsAndSaveAsync();

    public void Receive(DimmingMessage message) => this.IsDimmed = message.IsEnabled;

    private void UpdatePriorities()
    {
        for (int i = 0; i < this.Characters.Count; i++)
        {
            this.Characters[i].Character.Model.Priority = i;
        }
    }

    private async Task RefreshCharactersAsync()
    {
        await this.syncLock.WaitAsync();
        try
        {
            var (notDeletedCharacters, missingByCharacter) = await Task.Run(() =>
            {
                var inventory = this.inventoryService.GetInventory();
                var missing = this.inventoryService.CalculateMissingMaterials(inventory);
                var chars = inventory.NotDeletedCharacters.OrderBy(c => c.Priority).ToList();
                return (chars, missing);
            });

            var viewModels = await Task.Run(() =>
            {
                return notDeletedCharacters.Select(character =>
                {
                    var characterVm = new CharacterViewModel(character);
                    var materials = this.GetMaterials(characterVm, missingByCharacter);
                    var sorted = InventoryService.SortMaterialsForDisplay(materials);
                    return this.CreateCharacterViewModel(characterVm, sorted);
                }).ToList();
            });

            this.Characters.Clear();
            foreach (var vm in viewModels)
            {
                this.Characters.Add(vm);
            }
        }
        finally
        {
            this.syncLock.Release();
        }
    }

    private async Task RefreshMaterialsAndSaveAsync()
    {
        await this.syncLock.WaitAsync();
        try
        {
            var characterVms = this.Characters.ToList();

            await Task.Run(() =>
            {
                var inventory = this.inventoryService.GetInventory();
                var missingByCharacter = this.inventoryService.CalculateMissingMaterials(inventory);

                foreach (var charVm in characterVms)
                {
                    var materials = this.GetMaterials(charVm.Character, missingByCharacter);

                    charVm.RequiredMaterials = InventoryService.SortMaterialsForDisplay(materials);
                }
            });

            await this.dataIOService.SaveAsync();
        }
        finally
        {
            this.syncLock.Release();
        }
    }

    private List<MaterialRequirement> GetMaterials(CharacterViewModel character, Dictionary<Character, List<MaterialRequirement>> missingByCharacter)
    {
        if (!character.Activated)
        {
            return [.. this.inventoryService
                .TotalCost(character.Model)
                .Select(m => new MaterialRequirement(m, m.Amount)
                {
                    MissingAmount = m.Amount,
                    TakenFromInventory = 0,
                    CraftedAmount = 0,
                })];
        }

        return missingByCharacter.TryGetValue(character.Model, out var value) ? value : [];
    }

    private CharacterCardViewModel CreateCharacterViewModel(CharacterViewModel character, List<MaterialRequirement> materials)
    {
        return new CharacterCardViewModel(character, materials, this.dialogService, this.inventoryService, this.characterService);
    }

    private bool RemoveCharacter(Character character)
    {
        var vm = this.Characters.FirstOrDefault(c => c.Character.Model == character);
        if (vm is null)
        {
            return false;
        }

        this.Characters.Remove(vm);
        return true;
    }

    private async Task InitializeAsync()
    {
        await this.RefreshCharactersAsync();
        await this.RefreshMaterialsAndSaveAsync();
    }
}