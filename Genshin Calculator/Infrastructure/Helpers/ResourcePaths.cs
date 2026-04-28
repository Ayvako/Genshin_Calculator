using System;
using System.IO;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Presentation;

namespace Genshin_Calculator.Core.Helpers;

public static class ResourcePaths
{
    private static string embeddedPath = "Resources/Images";

    public static string ExternalBasePath { get; set; } = App.Configuration?["Paths:GameData"] ?? "Data/GameData";

    public static string ExternalPath { get; set; } = Path.Combine(ExternalBasePath, "Images");

    public static string EmbeddedPath
    {
        get => embeddedPath;
        set => embeddedPath = (value ?? string.Empty).TrimEnd('/', '\\');
    }

    public static Uri? Character(string name) => ToLocalFileUri("Characters", $"{name}.png");

    public static Uri? Material(string name) => ToLocalFileUri("Materials", $"{name}.png");

    public static Uri Tool(string name) => ToPackUri($"{EmbeddedPath}/Tools/{name}.png");

    public static Uri Element(Element name) => ToPackUri($"{EmbeddedPath}/Elements/{name}.png");

    public static Uri Weapon(WeaponType name) => ToPackUri($"{EmbeddedPath}/Weapons/{name}.png");

    public static Uri Star(MaterialRarity name) => ToPackUri($"{EmbeddedPath}/Stars/{name}.png");

    private static Uri? ToLocalFileUri(string folder, string fileName)
    {
        string fullPath = Path.GetFullPath(Path.Combine(ExternalPath, folder, fileName));

        if (!File.Exists(fullPath))
        {
            return ToPackUri($"{embeddedPath}/Placeholder.png");
        }

        return new Uri(fullPath, UriKind.Absolute);
    }

    private static Uri ToPackUri(string relativePath)
    {
        var cleaned = relativePath?.Replace("\\", "/").TrimStart('/');
        return new Uri($"pack://application:,,,/{cleaned}", UriKind.Absolute);
    }
}