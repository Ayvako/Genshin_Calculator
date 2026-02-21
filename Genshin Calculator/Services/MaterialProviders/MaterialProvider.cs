using Genshin_Calculator.Helpers;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace Genshin_Calculator.Services.MaterialProviders;

public abstract class MaterialProvider<TKey> : IMaterialProvider
    where TKey : notnull
{
    private readonly Dictionary<TKey, string[]> materials;

    protected MaterialProvider(string jsonName)
    {
        materials = GetMaterials<Dictionary<TKey, string[]>>(jsonName)
            ?? throw new InvalidOperationException($"{jsonName}.json not found");
    }

    public static T? GetMaterials<T>(string materials)
    {
        var uri = ResourcePaths.MaterialsJson(materials);

        StreamResourceInfo? info = Application.GetResourceStream(uri);
        if (info == null)
        {
            return default;
        }

        using var reader = new StreamReader(info.Stream);
        var json = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<T>(json);
    }

    public IEnumerable<string> GetMaterialGroup(Character character)
    {
        var key = GetKey(character);
        if (materials.TryGetValue(key, out var group))
        {
            return group;
        }

        return [];
    }

    public string GetMaterial(Character character, MaterialRarity rarity)
    {
        var key = GetKey(character);

        if (!materials.TryGetValue(key, out var materialSet))
        {
            throw new KeyNotFoundException($"Material group '{key}' not found");
        }

        return Resolve(materialSet, rarity);
    }

    protected abstract TKey GetKey(Character character);

    protected abstract string Resolve(string[] materials, MaterialRarity rarity);
}