using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using System;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.MaterialProviders;

public sealed class SkillMaterialProvider : IMaterialProvider
{
    public MaterialTypes SupportedType => MaterialTypes.SkillMaterial;

    public string GetMaterial(Character character, MaterialRarity rarity)
    {
        var names = MaterialNaming.GetSkillNames(GetBaseName(character));
        return rarity switch
        {
            MaterialRarity.Green => names[0],
            MaterialRarity.Blue => names[1],
            MaterialRarity.Violet => names[2],
            _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
        };
    }

    public IEnumerable<string> GetMaterialGroup(Character character) => MaterialNaming.GetSkillNames(GetBaseName(character));

    private static string GetBaseName(Character character)
    {
        return character.Assets?.SkillMaterials ?? throw new ArgumentException("Character has no skillMaterials");
    }
}