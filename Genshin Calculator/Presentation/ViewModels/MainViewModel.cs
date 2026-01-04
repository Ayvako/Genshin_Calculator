using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly InventoryService inventoryService;

    private readonly CharacterService characterService;

    public MainViewModel(InventoryService inventoryService, CharacterService characterService)
    {
        this.inventoryService = inventoryService;
        this.characterService = characterService;

        characterService.CharacterAdded += OnCharacterAdded;
        characterService.CharacterDeleted += OnCharacterDeleted;

        RefreshCharacters();
    }

    public ObservableCollection<CharacterCardViewModel> Characters { get; set; } = [];

    private void RefreshCharacters()
    {
        Characters.Clear();
        Inventory inventory = inventoryService.GetInventory();
        var missingByCharacter = inventoryService.CalculateMissingMaterials(inventory);

        foreach (var character in inventory.ActiveCharacters)
        {
            missingByCharacter.TryGetValue(character, out var materials);
            var required = materials ?? [];

            var charVm = new CharacterCardViewModel(character, required, characterService);
            charVm.Edited += RefreshAllMaterials;

            Characters.Add(charVm);
        }
    }

    private void OnCharacterAdded(Character character)
    {
        RefreshCharacters();
    }

    private void OnCharacterDeleted(Character character)
    {
        RefreshCharacters();
    }

    private void RefreshAllMaterials()
    {
        Inventory inventory = inventoryService.GetInventory();
        var missingByCharacter = inventoryService.CalculateMissingMaterials(inventory);

        foreach (var charVm in Characters)
        {
            missingByCharacter.TryGetValue(charVm.Character, out var materials);
            charVm.RequiredMaterials = materials ?? [];
        }
    }
}