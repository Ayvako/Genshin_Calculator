using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation;
using Genshin_Calculator.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Genshin_Calculator.Services;

public class DataIOService
{
    private readonly IInventoryStore store;

    private readonly string exportFilePath = App.Configuration["Paths:ExportFile"] ?? "Data/Export.json";

    private readonly string initFilesPath = App.Configuration["Paths:InitFiles"] ?? "pack://application:,,,/Resources/Json";

    public DataIOService(IInventoryStore store)
    {
        this.store = store;
    }

    public void Import()
    {
        var inventoryMaterials = new List<Material>();

        try
        {
            var mainData = this.LoadJson("Initializations.json");
            if (mainData["Materials"] is JObject simpleGroups)
            {
                AddSimpleGroup(inventoryMaterials, simpleGroups, "LocalSpecialty", MaterialTypes.LocalSpecialty, MaterialRarity.White);
                AddSimpleGroup(inventoryMaterials, simpleGroups, "MiniBoss", MaterialTypes.MiniBoss, MaterialRarity.Violet);
                AddSimpleGroup(inventoryMaterials, simpleGroups, "WeeklyBoss", MaterialTypes.WeeklyBoss, MaterialRarity.Orange);
                AddSimpleGroup(inventoryMaterials, simpleGroups, "Other", MaterialTypes.Other, MaterialRarity.Orange);
                AddSimpleGroup(inventoryMaterials, simpleGroups, "Mora", MaterialTypes.Mora, MaterialRarity.Blue);
            }

            this.LoadTieredGroup(inventoryMaterials, "Exp.json", MaterialTypes.Exp, [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet]);

            this.LoadTieredGroup(inventoryMaterials, "SkillMaterials.json", MaterialTypes.Book, [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet]);

            this.LoadTieredGroup(inventoryMaterials, "Enemies.json", MaterialTypes.Enemy, [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue]);

            this.LoadTieredGroup(inventoryMaterials, "Gems.json", MaterialTypes.Gem, [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet, MaterialRarity.Orange]);

            this.store.Inventory.Materials = inventoryMaterials;

            this.LoadCharacters();

            this.LoadUserExport();

            Console.WriteLine("✅ Import completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Import Error: {ex.Message}");
            throw;
        }
    }

    public void Export(Inventory inventory, List<Character> characters)
    {
        var exportJson = new JObject
        {
            ["Inventory"] = JToken.FromObject(inventory),
            ["Characters"] = JToken.FromObject(characters),
        };

        var directory = Path.GetDirectoryName(this.exportFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(this.exportFilePath, exportJson.ToString(Formatting.Indented));
        Console.WriteLine($"💾 Export saved to {this.exportFilePath}");
    }

    public void Save()
    {
        this.Export(this.store.Inventory, this.store.Inventory.Characters);
    }

    private static void UpdateCharacters(List<Character> baseChars, List<Character> importedChars)
    {
        var importMap = importedChars.ToDictionary(c => c.Name);

        foreach (var c in baseChars)
        {
            if (importMap.TryGetValue(c.Name, out var update))
            {
                c.ApplyChangesFrom(update);
            }
        }
    }

    private static void MergeInventories(Inventory baseInv, Inventory imported)
    {
        var importedDict = imported.Materials.ToDictionary(m => m.Name);

        foreach (var baseMat in baseInv.Materials)
        {
            if (importedDict.TryGetValue(baseMat.Name, out var importedMat))
            {
                baseMat.Amount = importedMat.Amount;
            }
        }

        baseInv.RefreshCache();
    }

    private static void AddSimpleGroup(List<Material> list, JObject source, string key, MaterialTypes type, MaterialRarity rarity)
    {
        if (source[key] is JArray names)
        {
            foreach (var name in names)
            {
                list.Add(new Material(name.ToString(), type, rarity, 0));
            }
        }
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

    private void LoadCharacters()
    {
        var json = this.LoadJson("Characters.json");
        var assets = json["Characters"]?.ToObject<List<Assets>>()
                     ?? throw new InvalidOperationException("Characters section missing");

        this.store.Inventory.Characters = [.. assets.Select(a => new Character(a.Name, a))];
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

    private void LoadUserExport()
    {
        if (!File.Exists(this.exportFilePath))
        {
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(this.exportFilePath);
            var exportJson = JObject.Parse(jsonContent);

            if (exportJson["Inventory"] is not null)
            {
                var importedInventory = exportJson["Inventory"]?.ToObject<Inventory>();
                if (importedInventory != null)
                {
                    MergeInventories(this.store.Inventory, importedInventory);
                }
            }

            if (exportJson["Characters"] is not null)
            {
                var importedChars = exportJson["Characters"]?.ToObject<List<Character>>() ?? [];
                UpdateCharacters(this.store.Inventory.Characters, importedChars);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning: Failed to load user export. {ex.Message}");
        }
    }
}