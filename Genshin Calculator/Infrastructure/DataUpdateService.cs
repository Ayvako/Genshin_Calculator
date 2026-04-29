using Genshin_Calculator.Models;
using Microsoft.Extensions.Configuration;
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

    private readonly HttpClient httpClient;

    private readonly string localBase;

    public DataUpdateService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        this.httpClient = httpClientFactory.CreateClient();
        this.localBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config["Paths:GameData"] ?? "Data/GameData");
    }

    public async Task UpdateAllDataAsync(IProgress<(string Message, double Percent)>? progress = null)
    {
        try
        {
            progress?.Report(("Syncing characters data...", 10));
            await Task.Delay(200);

            await this.UpdateJsonFile("Json/Characters.json");

            progress?.Report(("Syncing enemies data...", 20));
            await Task.Delay(200);
            await this.UpdateJsonFile("Json/Enemies.json");

            progress?.Report(("Syncing material images...", 21));
            await Task.Delay(200);
            await this.SyncFolderViaApi("Images/Materials", progress, fromPercent: 21, toPercent: 70);

            progress?.Report(("Syncing character images...", 71));
            await Task.Delay(200);
            await this.SyncImagesFromJson("Json/Characters.json", progress, fromPercent: 71, toPercent: 95);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Update: {ex.Message}");
        }
    }

    private async Task SyncImagesFromJson(
    string jsonRelativePath,
    IProgress<(string Message, double Percent)>? progress = null,
    double fromPercent = 0,
    double toPercent = 100)
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

        var toDownload = new List<(string Name, string Url, string LocalPath)>();

        foreach (var character in characters)
        {
            string name = character["Name"]?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            string imgRelative = $"Images/Characters/{name}.png";
            string localImgPath = Path.Combine(this.localBase, imgRelative);

            if (!File.Exists(localImgPath))
            {
                toDownload.Add((name, $"{GitHubRawBase}/{imgRelative}", localImgPath));
            }
        }

        if (toDownload.Count == 0)
        {
            progress?.Report(($"All characters are up to date...", toPercent));
            await Task.Delay(200);
            return;
        }

        for (int i = 0; i < toDownload.Count; i++)
        {
            var (name, url, localPath) = toDownload[i];
            double percent = fromPercent + ((i + 1.0) / toDownload.Count * (toPercent - fromPercent));
            progress?.Report(($"Downloading: {name}.png ({i + 1}/{toDownload.Count})", percent));
            await this.DownloadFile(url, localPath);
        }
    }

    private async Task SyncFolderViaApi(string folderRelativePath, IProgress<(string Message, double Percent)>? progress = null, double fromPercent = 0, double toPercent = 100)
    {
        try
        {
            string apiUrl = $"https://api.github.com/repos/Ayvako/Genshin_Calculator/contents/Genshin%20Calculator/GameData/{folderRelativePath}";

            if (!this.httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                this.httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            }

            string response = await this.httpClient.GetStringAsync(apiUrl);
            var files = JArray.Parse(response);

            var filesToDownload = new List<(string FileName, string DownloadUrl, string LocalPath)>();

            foreach (var file in files)
            {
                if (file["type"]?.ToString() != "file")
                {
                    continue;
                }

                string fileName = file["name"]?.ToString();
                string downloadUrl = file["download_url"]?.ToString();

                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(downloadUrl))
                {
                    continue;
                }

                string localFilePath = Path.Combine(this.localBase, folderRelativePath, fileName);
                if (!File.Exists(localFilePath))
                {
                    filesToDownload.Add((fileName, downloadUrl, localFilePath));
                }
            }

            if (filesToDownload.Count == 0)
            {
                progress?.Report(("All materials are up to date...", toPercent));
                await Task.Delay(200);
                return;
            }

            for (int i = 0; i < filesToDownload.Count; i++)
            {
                var (fileName, downloadUrl, localPath) = filesToDownload[i];
                double percent = fromPercent + ((i + 1.0) / filesToDownload.Count * (toPercent - fromPercent));
                progress?.Report(($"Downloading: {fileName} ({i + 1}/{filesToDownload.Count})", percent));
                await this.DownloadFile(downloadUrl, localPath);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to sync folder {folderRelativePath}: {ex.Message}");
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