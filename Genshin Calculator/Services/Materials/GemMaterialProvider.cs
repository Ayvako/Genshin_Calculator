using System;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Materials;

public sealed class GemMaterialProvider : MaterialProvider<Element>
{
    public GemMaterialProvider()
        : base("Gems")
    {
    }

    protected override Element GetKey(Character character) =>
        character.Assets?.Element
        ?? throw new ArgumentException("Character has no element");

    protected override string Resolve(string[] materials, MaterialRarity rarity) => rarity switch
    {
        MaterialRarity.Green => materials[0],
        MaterialRarity.Blue => materials[1],
        MaterialRarity.Violet => materials[2],
        MaterialRarity.Orange => materials[3],
        _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
    };
}