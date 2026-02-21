using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation;
using Genshin_Calculator.Services.Interfaces;
using Genshin_Calculator.Services.MaterialProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

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
            this.LoadCharacters();
            var allCharacters = this.store.Inventory.Characters;

            var skillBaseNames = allCharacters
                .Select(c => c.Assets!.SkillMaterials)
                .Distinct()
                .Where(name => !string.IsNullOrEmpty(name));

            foreach (var baseName in skillBaseNames)
            {
                inventoryMaterials.Add(new Material($"TeachingsOf{baseName}", MaterialTypes.SkillMaterial, MaterialRarity.Green, 0));
                inventoryMaterials.Add(new Material($"GuideTo{baseName}", MaterialTypes.SkillMaterial, MaterialRarity.Blue, 0));
                inventoryMaterials.Add(new Material($"PhilosophiesOf{baseName}", MaterialTypes.SkillMaterial, MaterialRarity.Violet, 0));
            }

            var elements = allCharacters
                .Select(c => c.Assets!.Element)
                .Distinct();

            foreach (var element in elements)
            {
                string baseName = GemMaterialProvider.GetBaseGemName(element);
                inventoryMaterials.Add(new Material($"{baseName}Sliver", MaterialTypes.Gem, MaterialRarity.Green, 0));
                inventoryMaterials.Add(new Material($"{baseName}Fragment", MaterialTypes.Gem, MaterialRarity.Blue, 0));
                inventoryMaterials.Add(new Material($"{baseName}Chunk", MaterialTypes.Gem, MaterialRarity.Violet, 0));
                inventoryMaterials.Add(new Material($"{baseName}Gemstone", MaterialTypes.Gem, MaterialRarity.Orange, 0));
            }

            AddUniqueFromCharacters(inventoryMaterials, allCharacters, c => c.Assets!.LocalSpecialty, MaterialTypes.LocalSpecialty, MaterialRarity.White);
            AddUniqueFromCharacters(inventoryMaterials, allCharacters, c => c.Assets!.MiniBoss, MaterialTypes.MiniBoss, MaterialRarity.Violet);
            AddUniqueFromCharacters(inventoryMaterials, allCharacters, c => c.Assets!.WeeklyBoss, MaterialTypes.WeeklyBoss, MaterialRarity.Orange);

            this.LoadTieredGroup(inventoryMaterials, "Enemies.json", MaterialTypes.Enemy, [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue]);

            inventoryMaterials.Add(new Material("CrownOfInsight", MaterialTypes.Other, MaterialRarity.Blue, 0));
            inventoryMaterials.Add(new Material("Mora", MaterialTypes.Mora, MaterialRarity.Blue, 0));
            inventoryMaterials.Add(new Material("WanderersAdvice", MaterialTypes.Exp, MaterialRarity.Green, 0));
            inventoryMaterials.Add(new Material("AdventurersExperience", MaterialTypes.Exp, MaterialRarity.Blue, 0));
            inventoryMaterials.Add(new Material("HerosWit", MaterialTypes.Exp, MaterialRarity.Violet, 0));

            this.store.Inventory.Materials = inventoryMaterials;
            this.LoadUserExport();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Import Error: {ex.Message}");
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

    private static void AddUniqueFromCharacters(List<Material> list, IEnumerable<Character> characters, Func<Character, string> selector, MaterialTypes type, MaterialRarity rarity)
    {
        var names = characters.Select(selector).Distinct().Where(n => !string.IsNullOrEmpty(n));
        foreach (var name in names)
        {
            list.Add(new Material(name, type, rarity, 0));
        }
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