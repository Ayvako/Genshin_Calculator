using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly InventoryService inventoryService;

    private readonly CharacterService characterService;

    public MainViewModel(InventoryService inventoryService, CharacterService characterService)
    {
        this.inventoryService = inventoryService;
        this.characterService = characterService;

        characterService.CharacterAdded += this.OnCharacterAdded;
        characterService.CharacterDeleted += this.OnCharacterDeleted;

        this.RefreshCharacters();
    }

    public ObservableCollection<CharacterCardViewModel> Characters { get; set; } = [];

    private void RefreshCharacters()
    {
        this.Characters.Clear();
        Inventory inventory = this.inventoryService.GetInventory();
        var missingByCharacter = InventoryService.CalculateMissingMaterials(inventory);

        foreach (var character in inventory.ActiveCharacters)
        {
            missingByCharacter.TryGetValue(character, out var materials);
            var required = materials ?? [];

            var charVm = new CharacterCardViewModel(character, required, this.characterService);
            charVm.Edited += this.RefreshAllMaterials;

            this.Characters.Add(charVm);
        }
    }

    private void OnCharacterAdded(Character character)
    {
        this.RefreshCharacters();
    }

    private void OnCharacterDeleted(Character character)
    {
        this.RefreshCharacters();
    }

    private void RefreshAllMaterials()
    {
        Inventory inventory = this.inventoryService.GetInventory();
        var missingByCharacter = InventoryService.CalculateMissingMaterials(inventory);

        foreach (var charVm in this.Characters)
        {
            missingByCharacter.TryGetValue(charVm.Character, out var materials);
            charVm.RequiredMaterials = materials ?? [];
        }
    }
}