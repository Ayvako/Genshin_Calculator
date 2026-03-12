using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using Genshin_Calculator.Services.Interfaces;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.MaterialProviders;

public sealed class ExpMaterialProvider : IMaterialProvider
{
    private static readonly string[] ExpGroup =
    [
        "WanderersAdvice",
        "AdventurersExperience",
        "HerosWit"
    ];

    public string GetMaterial(Character character, MaterialRarity rarity) => rarity switch
    {
        MaterialRarity.Green => ExpGroup[0],
        MaterialRarity.Blue => ExpGroup[1],
        MaterialRarity.Violet => ExpGroup[2],
        _ => string.Empty,
    };

    public IEnumerable<string> GetMaterialGroup(Character character) => ExpGroup;
}