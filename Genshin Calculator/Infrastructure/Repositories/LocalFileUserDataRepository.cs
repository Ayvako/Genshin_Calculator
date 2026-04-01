using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        string tempFilePath = this.exportFilePath + ".tmp";
        string backupFilePath = this.exportFilePath + ".bak";
        string jsonString = exportJson.ToString(Formatting.Indented);

        try
        {
            using (FileStream fs = new(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
            using (StreamWriter sw = new(fs, Encoding.UTF8))
            {
                sw.Write(jsonString);
                sw.Flush();
                fs.Flush(true);
            }

            if (File.Exists(this.exportFilePath))
            {
                File.Replace(tempFilePath, this.exportFilePath, backupFilePath, ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempFilePath, this.exportFilePath);
            }

            Console.WriteLine($"💾 Export saved safely to {this.exportFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving export: {ex.Message}");

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    public (Inventory? Inventory, List<Character>? Characters) Load()
    {
        var result = TryLoadFile(this.exportFilePath);
        if (result.Inventory != null || result.Characters != null)
        {
            return result;
        }

        string backupFilePath = this.exportFilePath + ".bak";
        if (File.Exists(backupFilePath))
        {
            Console.WriteLine($"⚠️ Main file corrupted or missing. Attempting to load backup from {backupFilePath}");
            return TryLoadFile(backupFilePath);
        }

        return (null, null);
    }

    private static (Inventory? Inventory, List<Character>? Characters) TryLoadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return (null, null);
        }

        try
        {
            var jsonContent = File.ReadAllText(filePath);

            var exportJson = JObject.Parse(jsonContent);

            var importedInventory = exportJson["Inventory"]?.ToObject<Inventory>();
            var importedChars = exportJson["Characters"]?.ToObject<List<Character>>();

            return (importedInventory, importedChars);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning: Failed to load {filePath}. {ex.Message}");
            return (null, null);
        }
    }
}