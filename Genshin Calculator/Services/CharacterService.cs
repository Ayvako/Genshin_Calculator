using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services;

public class CharacterService
{
    private readonly InventoryService inventoryService;

    private readonly Dictionary<string, Character> characterByName;

    public CharacterService(InventoryService inventoryService)
    {
        this.inventoryService = inventoryService;

        this.characterByName = this.GetCharacters()
            .ToDictionary(c => c.Name.ToLowerInvariant(), c => c);
    }

    public void ChangePriority(Character character1, Character character2)
    {
        (character2.Priority, character1.Priority) = (character1.Priority, character2.Priority);
        this.UpdateCharacter(character1);
        this.UpdateCharacter(character2);
    }

    public void ToggleCharacterActivity(Character character)
    {
        character.Activated = !character.Activated;
        this.UpdateCharacter(character);
    }

    public void SetCharacterActivity(Character character, bool isActive)
    {
        character.Activated = isActive;
        this.UpdateCharacter(character);
    }

    public void AddCharacter(Character character)
    {
        character.Deleted = false;
        character.Activated = true;

        var allCharacters = this.GetCharacters();
        int maxPriority = allCharacters.Any()
            ? allCharacters.Max(c => c.Priority)
            : 0;

        character.Priority = maxPriority + 1;

        this.UpdateCharacter(character);
    }

    public void DeleteCharacter(Character character)
    {
        character.Deleted = true;
        character.Reset();
        this.UpdateCharacter(character);
    }

    public Character? GetCharacterByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        this.characterByName.TryGetValue(name.ToLowerInvariant(), out var character);

        return character;
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return this.inventoryService.GetCharacters();
    }

    public void UpdateCharacter(Character character)
    {
        WeakReferenceMessenger.Default.Send(new CharacterChangedMessage(character));
    }
}