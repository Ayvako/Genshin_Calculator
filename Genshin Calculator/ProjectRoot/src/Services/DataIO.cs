using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Resources;
using Genshin_Calculator.ProjectRoot.src.LevelingResources;
using Genshin_Calculator.ProjectRoot.Src.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Genshin_Calculator.ProjectRoot.Src.Services;

public static class DataIO
{
    private const string ExportFilePath = "Data/Export.json";
    private const string InitFilePath = "pack://application:,,,/Genshin Calculator;component/ProjectRoot/Resources/Json/Initializations.json";

    public static void Export(Inventory inventory, List<Character> characters)
    {

        var exportJson = new JObject
        {
            ["Inventory"] = JToken.FromObject(inventory),
            ["Characters"] = JToken.FromObject(characters)
        };

        Directory.CreateDirectory("Data");
        File.WriteAllText(ExportFilePath, exportJson.ToString(Formatting.Indented));

        Console.WriteLine("Export");
    }

    public static Inventory Import(out List<Character> characters)
    {
        Inventory inventory = new();
        characters = new();

        Uri resourceUri = new(InitFilePath);
        StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri)
            ?? throw new FileNotFoundException("Initializations.json not found in resources.");
        using StreamReader reader = new(resourceInfo.Stream);

        string jsonContent = reader.ReadToEnd();
        JObject initJson = JObject.Parse(jsonContent);

        var materials = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
            initJson["Materials"].ToString());

        var merged = MergeLists(
            materials["LocalSpecialty"],
            materials["BookType"],
            materials["Gem"],
            materials["Enemy"],
            materials["MiniBoss"],
            materials["WeeklyBoss"],
            materials["Other"]);

        inventory.Materials = merged.Select(m => new Material(m, MaterailTypes.OTHER, 0, 0)).ToList();

        var assets = JsonConvert.DeserializeObject<List<Assets>>(initJson["Characters"].ToString());
        foreach (var asset in assets)
            characters.Add(new Character(asset.Name, asset));

        if (File.Exists(ExportFilePath))
        {
            JObject exportJson = JObject.Parse(File.ReadAllText(ExportFilePath));

            if (exportJson["Inventory"] is not null)
            {
                var importedInventory = exportJson["Inventory"].ToObject<Inventory>();
                inventory = MergeInventories(inventory, importedInventory);
            }

            if (exportJson["Characters"] is not null)
            {
                var importedChars = exportJson["Characters"].ToObject<List<Character>>() ?? new();
                UpdateCharacters(characters, importedChars);
            }
        }
        inventory.Characters = characters;
        Console.WriteLine("✅ Import completed");
        return inventory;
    }

    private static void UpdateCharacters(List<Character> baseChars, List<Character> importedChars)
    {
        foreach (var c in baseChars)
        {
            var update = importedChars.FirstOrDefault(x => x.Name == c.Name);
            if (update == null) continue;

            c.Priority = update.Priority;
            c.CurrentLevel = update.CurrentLevel;
            c.DesiredLevel = update.DesiredLevel;
            c.AutoAttack = update.AutoAttack;
            c.Elemental = update.Elemental;
            c.Burst = update.Burst;
            c.Deleted = update.Deleted;
            c.Activated = update.Activated;
        }
    }

    private static Inventory MergeInventories(Inventory baseInv, Inventory imported)
    {
        foreach (var m in imported.Materials)
        {
            var existing = baseInv.GetMaterial(m.Name);
            if (existing == null)
                baseInv.Materials.Add(new Material(m.Name, m.Type, m.Rarity, m.Amount));
            else
                existing.Amount = m.Amount;
        }

        baseInv.RefreshCache();
        return baseInv;
    }

    private static List<string> MergeLists(params List<string>[] lists) =>
        lists.SelectMany(x => x).Distinct().ToList();

    public static Dictionary<string, string[]> GetMaterials(string materials)
    {
        Uri resourceUri = new($"pack://application:,,,/Genshin Calculator;component/Resources/Json/{materials}.json");
        StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri);
        using StreamReader reader = new(resourceInfo.Stream);
        string jsonContent = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<Dictionary<string, string[]>>(jsonContent);
    }
}