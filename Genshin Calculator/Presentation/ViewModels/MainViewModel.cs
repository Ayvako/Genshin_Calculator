using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<CharacterChangedMessage>, IRecipient<RefreshMaterialsRequestMessage>
{
    private readonly InventoryService inventoryService;

    private readonly CharacterService characterService;

    public MainViewModel(InventoryService inventoryService, CharacterService characterService)
    {
        this.inventoryService = inventoryService;
        this.characterService = characterService;

        this.IsActive = true;
        this.RefreshCharacters();
    }

    public ObservableCollection<CharacterCardViewModel> Characters { get; } = [];

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
    }

    public void Receive(RefreshMaterialsRequestMessage message)
    {
        this.RefreshAllMaterials();
    }

    private void RefreshCharacters()
    {
        this.Characters.Clear();
        var inventory = this.inventoryService.GetInventory();
        var missingByCharacter = this.inventoryService.CalculateMissingMaterials(inventory);

        foreach (var character in inventory.ActiveCharacters)
        {
            var materials = missingByCharacter.GetValueOrDefault(character) ?? [];
            var charVm = this.CreateCharacterViewModel(character, materials);
            this.Characters.Add(charVm);
        }
    }

    private CharacterCardViewModel CreateCharacterViewModel(Character character, List<Material> materials)
    {
        var charVm = new CharacterCardViewModel(character, materials, this.characterService);
        return charVm;
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