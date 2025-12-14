using System;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Materials;

public sealed class BookMaterialProvider : MaterialProvider<string>
{
    public BookMaterialProvider()
        : base("Books")
    {
    }

    protected override string GetKey(Character character) =>
        character.Assets?.BookType
        ?? throw new ArgumentException("Character has no book group");

    protected override string Resolve(string[] materials, MaterialRarity rarity) => rarity switch
    {
        MaterialRarity.Green => materials[0],
        MaterialRarity.Blue => materials[1],
        MaterialRarity.Violet => materials[2],
        _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
    };
}