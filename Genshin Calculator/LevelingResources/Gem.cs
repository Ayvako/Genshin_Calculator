using System;
using System.Collections.Generic;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.LevelingResources
{
    public static class Gem
    {
        private static readonly Dictionary<string, string[]> Gems = DataIOService.GetMaterials("Gems");

        public static string GetMaterial(Character character, MaterialRarity rarity)
        {
            if (character.Assets?.Element is null)
            {
                throw new ArgumentException("Character has no gem group defined.", nameof(character));
            }

            if (!Gems.TryGetValue(character.Assets.Element, out var materials))
            {
                throw new KeyNotFoundException($"Gem group '{character.Assets.Element}' not found.");
            }

            return rarity switch
            {
                MaterialRarity.Green => materials[0],
                MaterialRarity.Blue => materials[1],
                MaterialRarity.Violet => materials[2],
                MaterialRarity.Orange => materials[3],
                _ => throw new Exception($"Unknown rarity '{rarity}' for gem."),
            };
        }
    }
}