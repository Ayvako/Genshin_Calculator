using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Genshin_Calculator.Infrastructure;

public class DataUpdateService
{
    private const string GitHubRawBase = "https://raw.githubusercontent.com/Ayvako/Genshin_Calculator/master/Genshin%20Calculator/GameData";

    private readonly HttpClient httpClient = new();

    private readonly string localBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/GameData");

    public async Task UpdateAllDataAsync()
    {
        try
        {
            await this.UpdateJsonFile("Json/Characters.json");
            await this.UpdateJsonFile("Json/Enemies.json");

            await this.SyncImagesFromJson("Json/Characters.json", "Characters");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Update: {ex.Message}");
        }
    }

    private async Task UpdateJsonFile(string relativePath)
    {
        string url = $"{GitHubRawBase}/{relativePath}";
        string localPath = Path.Combine(this.localBase, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

        string content = await this.httpClient.GetStringAsync(url);
        await File.WriteAllTextAsync(localPath, content);
    }

    private async Task SyncImagesFromJson(string jsonRelativePath, string category)
    {
        string localJsonPath = Path.Combine(this.localBase, jsonRelativePath);
        if (!File.Exists(localJsonPath))
        {
            return;
        }

        var json = JObject.Parse(await File.ReadAllTextAsync(localJsonPath));
        var characters = json["Characters"]?.ToObject<List<JObject>>();

        if (characters == null)
        {
            return;
        }

        foreach (var character in characters)
        {
            string name = character["Name"]?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            string imgRelative = $"Images/{category}/{name}.png";
            string localImgPath = Path.Combine(this.localBase, imgRelative);

            if (!File.Exists(localImgPath))
            {
                await this.DownloadFile($"{GitHubRawBase}/{imgRelative}", localImgPath);
            }
        }
    }

    private async Task DownloadFile(string url, string localPath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
            byte[] data = await this.httpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(localPath, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to download file {url}: {ex.Message}");
        }
    }
}