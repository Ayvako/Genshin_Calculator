using System;
using System.Collections.Generic;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.LevelingResources;

public static class Enemy
{
    private static readonly Dictionary<string, string[]> Enemies = DataIOService.GetMaterials("Enemies");

    public static string GetMaterial(Character character, MaterialRarity rarity)
    {
        if (character.Assets?.Enemy is null)
        {
            throw new ArgumentException("Character has no enemy group defined.", nameof(character));
        }

        if (!Enemies.TryGetValue(character.Assets.Enemy, out var materials))
        {
            throw new KeyNotFoundException($"Enemy group '{character.Assets.Enemy}' not found.");
        }

        return rarity switch
        {
            MaterialRarity.White => materials[0],
            MaterialRarity.Green => materials[1],
            MaterialRarity.Blue => materials[2],
            _ => throw new Exception($"Unknown rarity '{rarity}' for gem."),
        };
    }
}