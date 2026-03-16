using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using System;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.MaterialProviders;

public sealed class SkillMaterialProvider : IMaterialProvider
{
    public string GetMaterial(Character character, MaterialRarity rarity)
    {
        var baseName = GetBaseName(character);

        return rarity switch
        {
            MaterialRarity.Green => $"TeachingsOf{baseName}",
            MaterialRarity.Blue => $"GuideTo{baseName}",
            MaterialRarity.Violet => $"PhilosophiesOf{baseName}",
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), "Talent books only have Green, Blue, and Violet rarities"),
        };
    }

    public IEnumerable<string> GetMaterialGroup(Character character)
    {
        var baseName = GetBaseName(character);

        return
        [
            $"TeachingsOf{baseName}",
            $"GuideTo{baseName}",
            $"PhilosophiesOf{baseName}"
        ];
    }

    private static string GetBaseName(Character character)
    {
        if (string.IsNullOrEmpty(character.Assets?.SkillMaterials))
        {
            throw new ArgumentException($"Character {character.Name} has no skill materials base name defined");
        }

        return character.Assets.SkillMaterials;
    }
}