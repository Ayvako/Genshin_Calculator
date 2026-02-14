using System;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Materials;

public sealed class SkillMaterialProvider : MaterialProvider<string>
{
    public SkillMaterialProvider()
        : base("SkillMaterials")
    {
    }

    protected override string GetKey(Character character) =>
        character.Assets?.SkillMaterials
        ?? throw new ArgumentException("Character has no skill materials group");

    protected override string Resolve(string[] materials, MaterialRarity rarity) => rarity switch
    {
        MaterialRarity.Green => materials[0],
        MaterialRarity.Blue => materials[1],
        MaterialRarity.Violet => materials[2],
        _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
    };
}