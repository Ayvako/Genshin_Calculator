using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Genshin_Calculator.Infrastructure.Repositories;

public class JsonGameDataRepository : IDataRepository
{
    private readonly string basePath;

    public JsonGameDataRepository(IConfiguration config)
    {
        this.basePath = config["Paths:GameData"] ?? "Data/GameData";
    }

    public List<Character> GetBaseCharacters()
    {
        var json = this.LoadJson("Characters.json");
        var assets = json["Characters"]?.ToObject<List<Assets>>()
                     ?? throw new InvalidOperationException("Characters section missing");

        return [.. assets.Select(a => new Character(a.Name, a))];
    }

    public List<Material> GetStaticMaterials()
    {
        var materials = new List<Material>();
        this.LoadTieredGroup(materials, "Enemies.json", MaterialTypes.Enemy, [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue]);
        return materials;
    }

    private JObject LoadJson(string fileName)
    {
        string filePath = Path.Combine(this.basePath, "Json", fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Static data file not found: {filePath}");
        }

        string jsonContent = File.ReadAllText(filePath);
        return JObject.Parse(jsonContent);
    }

    private void LoadTieredGroup(List<Material> targetList, string fileName, MaterialTypes type, MaterialRarity[] rarities)
    {
        var json = this.LoadJson(fileName);
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