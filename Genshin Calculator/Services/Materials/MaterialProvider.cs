using System;
using System.Collections.Generic;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Materials;

public abstract class MaterialProvider<TKey> : IMaterialProvider
    where TKey : notnull
{
    private readonly Dictionary<TKey, string[]> materials;

    protected MaterialProvider(string jsonName)
    {
        this.materials = DataIOService.GetMaterials<Dictionary<TKey, string[]>>(jsonName)
            ?? throw new InvalidOperationException($"{jsonName}.json not found");
    }

    public string GetMaterial(Character character, MaterialRarity rarity)
    {
        var key = this.GetKey(character);

        if (!this.materials.TryGetValue(key, out var materialSet))
        {
            throw new KeyNotFoundException($"Material group '{key}' not found");
        }

        return this.Resolve(materialSet, rarity);
    }

    protected abstract TKey GetKey(Character character);

    protected abstract string Resolve(string[] materials, MaterialRarity rarity);
}