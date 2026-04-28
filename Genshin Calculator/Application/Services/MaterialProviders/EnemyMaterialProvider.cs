using System;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public sealed class EnemyMaterialProvider : MaterialProvider<string>
{
    public EnemyMaterialProvider()
        : base("Enemies")
    {
    }

    public override MaterialTypes SupportedType => MaterialTypes.Enemy;

    protected override string GetKey(Character character) =>
        character.Assets?.Enemy
        ?? throw new ArgumentException("Character has no enemy group");

    protected override string Resolve(string[] materials, MaterialRarity rarity) => rarity switch
    {
        MaterialRarity.White => materials[0],
        MaterialRarity.Green => materials[1],
        MaterialRarity.Blue => materials[2],
        _ => throw new ArgumentOutOfRangeException(nameof(rarity)),
    };
}