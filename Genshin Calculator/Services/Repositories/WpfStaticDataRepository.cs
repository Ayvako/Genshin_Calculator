using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using Genshin_Calculator.Presentation;
using Genshin_Calculator.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Genshin_Calculator.Services.Repositories;

public class WpfStaticDataRepository : IStaticDataRepository
{
    private readonly string initFilesPath = App.Configuration["Paths:InitFiles"] ?? "pack://application:,,,/Resources/Json";

    public List<Character> GetBaseCharacters()
    {
        var json = this.LoadJson("Characters.json");
        var assets = json["Characters"]?.ToObject<List<Assets>>()
                     ?? throw new InvalidOperationException("Characters section missing");

        return assets.Select(a => new Character(a.Name, a)).ToList();
    }

    public List<Material> GetStaticMaterials()
    {
        var materials = new List<Material>();
        this.LoadTieredGroup(materials, "Enemies.json", MaterialTypes.Enemy, [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue]);
        return materials;
    }

    private JObject LoadJson(string fileName)
    {
        var fullPath = $"{this.initFilesPath.TrimEnd('/')}/{fileName}";
        Uri uri = new(fullPath);
        var info = Application.GetResourceStream(uri)
                   ?? throw new FileNotFoundException($"Resource not found: {fullPath}");

        using var reader = new StreamReader(info.Stream);
        return JObject.Parse(reader.ReadToEnd());
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