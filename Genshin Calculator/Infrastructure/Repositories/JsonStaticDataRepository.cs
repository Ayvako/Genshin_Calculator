using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Genshin_Calculator.Infrastructure.Repositories;

public class JsonStaticDataRepository : IStaticDataRepository
{
    public List<Character> GetBaseCharacters()
    {
        var json = LoadJson("Characters.json");
        var assets = json["Characters"]?.ToObject<List<Assets>>()
                     ?? throw new InvalidOperationException("Characters section missing");

        return [.. assets.Select(a => new Character(a.Name, a))];
    }

    public List<Material> GetStaticMaterials()
    {
        var materials = new List<Material>();
        LoadTieredGroup(materials, "Enemies.json", MaterialTypes.Enemy, [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue]);
        return materials;
    }

    private static JObject LoadJson(string fileName)
    {
        var assembly = typeof(JsonStaticDataRepository).Assembly;
        var resourceName = $"Genshin_Calculator.Resources.Json.{fileName}";

        using Stream? stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Resource not found: {resourceName}");
        using var reader = new StreamReader(stream);
        return JObject.Parse(reader.ReadToEnd());
    }

    private static void LoadTieredGroup(List<Material> targetList, string fileName, MaterialTypes type, MaterialRarity[] rarities)
    {
        var json = LoadJson(fileName);
        foreach (var property in json.Properties())
        {
            var names = property.Value.ToObject<string[]>();
            if (names == null)
            {
                continue;
            }

            int count = Math.Min(names.Length, rarities.Length);
            for (int i = 0; i < count; i++)
            {
                targetList.Add(new Material(names[i], type, rarities[i], 0));
            }
        }
    }
}