using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public sealed class GemMaterialProvider : IMaterialProvider
{
    public MaterialTypes SupportedType => MaterialTypes.Gem;

    public string GetMaterial(Character character, MaterialRarity rarity)
    {
        var names = MaterialNaming.GetGemNames(character.Assets?.Element ?? throw new ArgumentException("Character has no element"));
        return rarity switch
        {
            MaterialRarity.Green => names[0],
            MaterialRarity.Blue => names[1],
            MaterialRarity.Violet => names[2],
            MaterialRarity.Orange => names[3],
            _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
        };
    }

    public IEnumerable<string> GetMaterialGroup(Character character)
            => MaterialNaming.GetGemNames(character.Assets?.Element ?? throw new ArgumentException("Character has no element"));
}