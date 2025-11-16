using System;
using System.Collections.Generic;
using System.Linq;
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

    public event Action<Character>? CharacterUpdated;

    public event Action<Character>? CharacterAdded;

    public event Action<Character>? CharacterDeleted;

    public static void ChangePriority(Character character1, Character character2)
    {
        (character2.Priority, character1.Priority) = (character1.Priority, character2.Priority);
    }

    public void EnableCharacter(Character character)
    {
        character.Activated = true;
        this.CharacterUpdated?.Invoke(character);
    }

    public void DisableCharacter(Character character)
    {
        character.Activated = false;
        this.CharacterUpdated?.Invoke(character);
    }

    public void AddCharacter(Character character)
    {
        character.Deleted = false;
        this.CharacterAdded?.Invoke(character);
    }

    public void DeleteCharacter(Character character)
    {
        character.Deleted = true;
        this.CharacterDeleted?.Invoke(character);
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

    public List<Character> GetCharacters()
    {
        return this.inventoryService.GetCharacters();
    }
}