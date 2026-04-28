using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Models;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IInventoryService inventoryService;

    public CharacterService(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
    }

    public void UpdateCharacter(Character character)
    {
        WeakReferenceMessenger.Default.Send(new CharacterChangedMessage(character));
    }

    public void ToggleCharacterActivity(Character character)
    {
        character.Activated = !character.Activated;
        UpdateCharacter(character);
    }

    public void DeleteCharacter(Character character)
    {
        character.Deleted = true;
        character.Reset();
        UpdateCharacter(character);
    }

    public void AddCharacter(Character character)
    {
        character.Deleted = false;
        character.Activated = true;

        var allCharacters = GetCharacters();
        int maxPriority = allCharacters.Any()
            ? allCharacters.Max(c => c.Priority)
            : 0;

        character.Priority = maxPriority + 1;

        UpdateCharacter(character);
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return inventoryService.GetCharacters();
    }
}