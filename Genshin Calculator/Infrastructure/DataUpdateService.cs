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

    private const string GitHubApiBase = "https://api.github.com/repos/Ayvako/Genshin_Calculator/contents/Genshin%20Calculator/GameData";

    private const string NoInternet = "No internet connection, using local data...";

    private const int ProgressDelayMs = 200;

    private readonly HttpClient httpClient;

    private readonly string localBase;

    public DataUpdateService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        this.httpClient = httpClientFactory.CreateClient();
        this.httpClient.Timeout = TimeSpan.FromSeconds(10);
        this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "C# App");
        this.localBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config["Paths:GameData"] ?? "Data/GameData");
    }

    public async Task UpdateAllDataAsync(IProgress<(string Message, double Percent)>? progress = null)
    {
        try
        {
            progress?.Report(("Syncing characters data...", 10));
            await Task.Delay(ProgressDelayMs);
            await this.UpdateJsonFileAsync("Json/Characters.json", progress, 20);

            progress?.Report(("Syncing enemies data...", 20));
            await Task.Delay(ProgressDelayMs);
            await this.UpdateJsonFileAsync("Json/Enemies.json", progress, 21);

            progress?.Report(("Syncing material images...", 21));
            await Task.Delay(ProgressDelayMs);
            await this.SyncFolderViaApiAsync("Images/Materials", progress, fromPercent: 21, toPercent: 70);

            progress?.Report(("Syncing character images...", 71));
            await Task.Delay(ProgressDelayMs);
            await this.SyncImagesFromJsonAsync("Json/Characters.json", progress, fromPercent: 71, toPercent: 95);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error Update: {ex.Message}");
        }
    }

    private async Task SyncImagesFromJsonAsync(string jsonRelativePath, IProgress<(string Message, double Percent)>? progress, double fromPercent, double toPercent)
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

        var toDownload = new List<(string Label, string Url, string LocalPath)>();

        foreach (var character in characters)
        {
            string? name = character["Name"]?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            string imgRelative = $"Images/Characters/{name}.png";
            string localImgPath = Path.Combine(this.localBase, imgRelative);

            if (!File.Exists(localImgPath))
            {
                toDownload.Add(($"{name}.png", $"{GitHubRawBase}/{imgRelative}", localImgPath));
            }
        }

        if (toDownload.Count == 0)
        {
            progress?.Report(("All characters are up to date...", toPercent));
            return;
        }

        await this.DownloadBatchAsync(toDownload, progress, fromPercent, toPercent);
    }

    private async Task SyncFolderViaApiAsync(string folderRelativePath, IProgress<(string Message, double Percent)>? progress, double fromPercent, double toPercent)
    {
        try
        {
            string apiUrl = $"{GitHubApiBase}/{folderRelativePath}";
            string response = await this.httpClient.GetStringAsync(apiUrl);
            var files = JArray.Parse(response);

            var toDownload = new List<(string Label, string Url, string LocalPath)>();

            foreach (var file in files)
            {
                if (file["type"]?.ToString() != "file")
                {
                    continue;
                }

                string? fileName = file["name"]?.ToString();
                string? downloadUrl = file["download_url"]?.ToString();

                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(downloadUrl))
                {
                    continue;
                }

                string localFilePath = Path.Combine(this.localBase, folderRelativePath, fileName);
                if (!File.Exists(localFilePath))
                {
                    toDownload.Add((fileName, downloadUrl, localFilePath));
                }
            }

            if (toDownload.Count == 0)
            {
                progress?.Report(("All materials are up to date...", toPercent));
                return;
            }

            await this.DownloadBatchAsync(toDownload, progress, fromPercent, toPercent);
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"No internet, skipping folder {folderRelativePath}: {ex.Message}");
            progress?.Report((NoInternet, toPercent));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to sync folder {folderRelativePath}: {ex.Message}");
        }
    }

    private async Task UpdateJsonFileAsync(string relativePath, IProgress<(string Message, double Percent)>? progress, double percent)
    {
        string url = $"{GitHubRawBase}/{relativePath}";
        string localPath = Path.Combine(this.localBase, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

        try
        {
            string content = await this.httpClient.GetStringAsync(url);
            await File.WriteAllTextAsync(localPath, content);
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"No internet, skipping {relativePath}: {ex.Message}");
            progress?.Report((NoInternet, percent));
        }
    }

    private async Task DownloadBatchAsync(List<(string Label, string Url, string LocalPath)> items, IProgress<(string Message, double Percent)>? progress, double fromPercent, double toPercent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var (label, url, localPath) = items[i];
            double percent = fromPercent + ((i + 1.0) / items.Count * (toPercent - fromPercent));
            progress?.Report(($"Downloading: {label} ({i + 1}/{items.Count})", percent));
            await this.DownloadFileAsync(url, localPath);
        }
    }

    private async Task DownloadFileAsync(string url, string localPath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
            byte[] data = await this.httpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(localPath, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to download {url}: {ex.Message}");
        }
    }
}