using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public abstract class MaterialProvider<TKey> : IMaterialProvider
    where TKey : notnull
{
    private readonly Dictionary<TKey, string[]> materials;

    private readonly string basePath = App.Configuration["Paths:GameData"] ?? "Data/GameData";

    protected MaterialProvider(string jsonName)
    {
        materials = LoadFileJson<Dictionary<TKey, string[]>>($"{jsonName}.json")
                    ?? throw new InvalidOperationException($"Файл {jsonName}.json не найден или пуст");
    }

    public virtual MaterialTypes SupportedType { get; }

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

    private T? LoadFileJson<T>(string fileName)
    {
        var filePath = Path.Combine(basePath, "Json", fileName);

        if (!File.Exists(filePath))
        {
            return default;
        }

        var jsonContent = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(jsonContent);
    }
}