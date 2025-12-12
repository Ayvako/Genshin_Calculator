using System;
using System.Collections.Generic;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Models.LevelingResources;

public static class Book
{
    // Later Add interace for setvices to get materials
    private static readonly Dictionary<string, string[]> Books = DataIOService.GetMaterials("Books")!;

    public static string GetMaterial(Character character, MaterialRarity rarity)
    {
        if (character.Assets?.BookType is null)
        {
            throw new ArgumentException("Character has no book group defined.", nameof(character));
        }

        if (!Books.TryGetValue(character.Assets.BookType, out var materials))
        {
            throw new KeyNotFoundException($"Book group '{character.Assets.BookType}' not found.");
        }

        return rarity switch
        {
            MaterialRarity.Green => materials[0],
            MaterialRarity.Blue => materials[1],
            MaterialRarity.Violet => materials[2],
            _ => throw new ArgumentOutOfRangeException($"Unknown rarity '{rarity}' for gem."),
        };
    }
}