using System;
using System.Collections.Generic;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.LevelingResources;

public static class Book
{
    private static readonly Dictionary<string, string[]>? Books = DataIOService.GetMaterials("Books");

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

        int index = (int)rarity;

        if (index >= materials.Length)
        {
            throw new InvalidOperationException(
                $"Book group '{character.Assets.BookType}' does not define material for rarity {rarity}.");
        }

        return materials[index];
    }
}