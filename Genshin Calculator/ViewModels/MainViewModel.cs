using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly InventoryService inventoryService;

    public MainViewModel(InventoryService inventoryService, CharacterService characterService)
    {
        this.inventoryService = inventoryService;

        characterService.CharacterAdded += this.OnCharacterAdded;

        this.RefreshCharacters();
    }

    public ObservableCollection<CharacterCardViewModel> Characters { get; set; } = [];

    private void RefreshCharacters()
    {
        this.Characters.Clear();

        Inventory inventory = this.inventoryService.GetInventory();
        foreach (var character in inventory.ActiveCharacters)
        {
            this.Characters.Add(new CharacterCardViewModel(character));
        }
    }

    private void OnCharacterAdded(Character character)
    {
        this.Characters.Add(new CharacterCardViewModel(character));
    }
}