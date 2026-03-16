using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Genshin_Calculator.Infrastructure.Repositories;

public class LocalFileUserDataRepository : IUserDataRepository
{
    private readonly string exportFilePath = App.Configuration["Paths:ExportFile"] ?? "Data/Export.json";

    public void Save(Inventory inventory, List<Character> characters)
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

    public (Inventory? Inventory, List<Character>? Characters) Load()
    {
        if (!File.Exists(this.exportFilePath))
        {
            return (null, null);
        }

        try
        {
            var jsonContent = File.ReadAllText(this.exportFilePath);
            var exportJson = JObject.Parse(jsonContent);

            var importedInventory = exportJson["Inventory"]?.ToObject<Inventory>();
            var importedChars = exportJson["Characters"]?.ToObject<List<Character>>();

            return (importedInventory, importedChars);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning: Failed to load user export. {ex.Message}");
            return (null, null);
        }
    }
}