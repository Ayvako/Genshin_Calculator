using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Genshin_Calculator.Services.MaterialProviders;

public abstract class MaterialProvider<TKey> : IMaterialProvider
    where TKey : notnull
{
    private readonly Dictionary<TKey, string[]> materials;

    protected MaterialProvider(string jsonName)
    {
        var resourceName = ResourcePaths.MaterialsJson(jsonName);
        this.materials = LoadEmbeddedJson<Dictionary<TKey, string[]>>(resourceName)
                    ?? throw new InvalidOperationException($"{jsonName}.json not found");
    }

    public virtual MaterialTypes SupportedType { get; }

    public IEnumerable<string> GetMaterialGroup(Character character)
    {
        var key = this.GetKey(character);
        if (this.materials.TryGetValue(key, out var group))
        {
            return group;
        }

        return [];
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

    private static T? LoadEmbeddedJson<T>(string resourceName)
    {
        var assembly = typeof(MaterialProvider<TKey>).Assembly;

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return default;
        }

        using var reader = new StreamReader(stream);
        return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
    }
}