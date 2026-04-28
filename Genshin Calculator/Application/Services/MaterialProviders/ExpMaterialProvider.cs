using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public sealed class ExpMaterialProvider : IMaterialProvider
{
    private static readonly string[] ExpGroup =
    [
        "WanderersAdvice",
        "AdventurersExperience",
        "HerosWit"
    ];

    public MaterialTypes SupportedType => MaterialTypes.Exp;

    public string GetMaterial(Character character, MaterialRarity rarity) => rarity switch
    {
        MaterialRarity.Green => ExpGroup[0],
        MaterialRarity.Blue => ExpGroup[1],
        MaterialRarity.Violet => ExpGroup[2],
        _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
    };

    public IEnumerable<string> GetMaterialGroup(Character character) => ExpGroup;
}