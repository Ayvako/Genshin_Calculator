using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Resources;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation;
using Genshin_Calculator.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Genshin_Calculator.Services;

public class DataIOService
{
    private const string Characters = "Characters";

    private static readonly string ExportFilePath = App.Configuration["Paths:ExportFile"]!;

    private static readonly string InitFilePath = App.Configuration["Paths:InitFile"]!;

    private readonly IInventoryStore store;

    public DataIOService(IInventoryStore store)
    {
        this.store = store;
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

    public static void Export(Inventory inventory, List<Character> characters)
    {
        var exportJson = new JObject
        {
            ["Inventory"] = JToken.FromObject(inventory),
            [Characters] = JToken.FromObject(characters),
        };

        Directory.CreateDirectory("Data");
        File.WriteAllText(ExportFilePath, exportJson.ToString(Formatting.Indented));

        Console.WriteLine("Export");
    }

    public void Import()
    {
        var characters = this.store.Inventory.Characters;
        Uri resourceUri = new(InitFilePath);
        StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri)
            ?? throw new FileNotFoundException("Initializations.json not found in resources.");
        using StreamReader reader = new(resourceInfo.Stream);

        string jsonContent = reader.ReadToEnd();
        JObject initJson = JObject.Parse(jsonContent);
        ArgumentNullException.ThrowIfNull(initJson);

        var materials = initJson["Materials"]?.ToObject<Dictionary<string, List<string>>>()
            ?? throw new InvalidOperationException("Materials section is invalid or missing");

        var merged = MergeLists(
            materials["LocalSpecialty"],
            materials["MiniBoss"],
            materials["WeeklyBoss"],
            materials["Other"]);

        this.store.Inventory.Materials = [.. merged.Select(m => new Material(m, MaterialTypes.Other, 0, 0))];

        var characterAssets = initJson[Characters]?.ToObject<List<Assets>>()
            ?? throw new InvalidOperationException("Characters section is missing or invalid");
        foreach (var asset in characterAssets)
        {
            characters.Add(new Character(asset.Name, asset));
        }

        if (File.Exists(ExportFilePath))
        {
            JObject exportJson = JObject.Parse(File.ReadAllText(ExportFilePath));

            if (exportJson["Inventory"] is not null)
            {
                var importedInventory = exportJson["Inventory"]?.ToObject<Inventory>()
                    ?? throw new InvalidOperationException("Failed to deserialize Inventory");
                this.store.Inventory = MergeInventories(this.store.Inventory, importedInventory);
            }

            if (exportJson[Characters] is not null)
            {
                var importedChars = exportJson[Characters]?.ToObject<List<Character>>()
                    ?? [];

                UpdateCharacters(characters, importedChars);
            }
        }

        Console.WriteLine("✅ Import completed");
    }

    private static void UpdateCharacters(List<Character> baseChars, List<Character> importedChars)
    {
        foreach (var c in baseChars)
        {
            var update = importedChars.FirstOrDefault(x => x.Name == c.Name);
            if (update == null)
            {
                continue;
            }

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
            {
                baseInv.Materials.Add(new Material(m.Name, m.Type, m.Rarity, m.Amount));
            }
            else
            {
                existing.Amount = m.Amount;
            }
        }

        baseInv.RefreshCache();
        return baseInv;
    }

    private static List<string> MergeLists(params List<string>[] lists) =>
        [.. lists.SelectMany(x => x).Distinct()];
}