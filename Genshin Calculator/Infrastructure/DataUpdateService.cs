using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Genshin_Calculator.Infrastructure;

public class DataUpdateService
{

    private const string NoInternet = "No internet connection, using local data...";

    private readonly HttpClient httpClient;

    private readonly string localBase;

    private readonly string apiUrl;

    private readonly string zipUrl;

    private readonly string targetPrefix;

    public DataUpdateService(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        this.httpClient = httpClientFactory.CreateClient();
        this.httpClient.Timeout = TimeSpan.FromSeconds(10);
        this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "C# App");

        this.localBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config["Paths:GameData"] ?? "Data/GameData");

        string owner = config["GitHubSettings:Owner"] ?? "Ayvako";
        string repo = config["GitHubSettings:Repo"] ?? "Genshin_Calculator";
        string branch = config["GitHubSettings:Branch"] ?? "master";
        string internalPath = config["GitHubSettings:InternalPath"] ?? "Genshin Calculator/GameData";

        this.apiUrl = $"https://api.github.com/repos/{owner}/{repo}/branches/{branch}";
        this.zipUrl = $"https://github.com/{owner}/{repo}/archive/refs/heads/{branch}.zip";

        this.targetPrefix = $"{repo}-{branch}/{internalPath}/";
    }

    public async Task UpdateAllDataAsync(IProgress<(string Message, double Percent)>? progress = null, double fromPercent = 0, double toPercent = 100)
    {
        try
        {
            string? onlineSha = await this.GetLatestCommitShaAsync();

            string versionFile = Path.Combine(this.localBase, "version.txt");
            string localSha = File.Exists(versionFile) ? await File.ReadAllTextAsync(versionFile) : string.Empty;

            if (!string.IsNullOrEmpty(onlineSha) && onlineSha == localSha)
            {
                progress?.Report(("Data is already up to date.", toPercent));
                return;
            }

            await this.DownloadAndExtractZipAsync(progress, fromPercent, toPercent);

            if (!string.IsNullOrEmpty(onlineSha))
            {
                await File.WriteAllTextAsync(versionFile, onlineSha);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Update failed: {ex.Message}");
            progress?.Report((NoInternet, toPercent));
        }
    }

    private async Task DownloadAndExtractZipAsync(IProgress<(string Message, double Percent)>? progress, double from, double to)
    {
        try
        {
            progress?.Report(("Downloading updates...", from + ((to - from) * 0.1)));

            using Stream stream = await this.httpClient.GetStreamAsync(this.zipUrl);
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);

            var entries = archive.Entries
                .Where(e => e.FullName.StartsWith(this.targetPrefix) && !string.IsNullOrEmpty(e.Name))
                .ToList();

            if (entries.Count == 0)
            {
                Debug.WriteLine($"No files found for prefix: {this.targetPrefix}");
                return;
            }

            int completed = 0;
            double range = to - from;
            foreach (var entry in entries)
            {
                string relativePath = entry.FullName.Substring(this.targetPrefix.Length);
                string destinationPath = Path.Combine(this.localBase, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                entry.ExtractToFile(destinationPath, overwrite: true);

                completed++;
                double percent = from + (range * completed / entries.Count);
                progress?.Report(($"Extracting: {entry.Name}", percent));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Download/Extract error: {ex.Message}");
            throw;
        }
    }

    private async Task<string?> GetLatestCommitShaAsync()
    {
        try
        {
            var response = await this.httpClient.GetStringAsync(this.apiUrl);
            var json = Newtonsoft.Json.Linq.JObject.Parse(response);
            return json["commit"]?["sha"]?.ToString();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to fetch version: {ex.Message}");
            return null;
        }
    }
}