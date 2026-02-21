using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Messages;

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

    public static void UpdateCharacter(Character character)
    {
        WeakReferenceMessenger.Default.Send(new CharacterChangedMessage(character));
    }

    public static void ChangePriority(Character character1, Character character2)
    {
        (character2.Priority, character1.Priority) = (character1.Priority, character2.Priority);
        UpdateCharacter(character1);
        UpdateCharacter(character2);
    }

    public static void ToggleCharacterActivity(Character character)
    {
        character.Activated = !character.Activated;
        UpdateCharacter(character);
    }

    public static void SetCharacterActivity(Character character, bool isActive)
    {
        character.Activated = isActive;
        UpdateCharacter(character);
    }

    public static void DeleteCharacter(Character character)
    {
        character.Deleted = true;
        character.Reset();
        UpdateCharacter(character);
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

        UpdateCharacter(character);
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
}