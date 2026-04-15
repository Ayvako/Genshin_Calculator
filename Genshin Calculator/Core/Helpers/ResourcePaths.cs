using System;
using System.IO;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Presentation;

namespace Genshin_Calculator.Core.Helpers;

public static class ResourcePaths
{
    private static string basePath = "Resources/Images";

    public static string ExternalImagesPath { get; set; } = App.Configuration?["Paths:StaticImages"] ?? "Data/Static/Images";

    public static string BasePath
    {
        get => basePath;
        set => basePath = (value ?? string.Empty).TrimEnd('/', '\\');
    }

    public static Uri? Character(string name) => ToLocalFileUri("Characters", $"{name}.png");

    public static Uri? Material(string name) => ToLocalFileUri("Materials", $"{name}.png");

    public static Uri Tool(string name) => ToPackUri($"{BasePath}/Tools/{name}.png");

    public static Uri Element(Element name) => ToPackUri($"{BasePath}/Elements/{name}.png");

    public static Uri Weapon(WeaponType name) => ToPackUri($"{BasePath}/Weapons/{name}.png");

    public static Uri Star(MaterialRarity name) => ToPackUri($"{BasePath}/Stars/{name}.png");

    private static Uri? ToLocalFileUri(string folder, string fileName)
    {
        string fullPath = Path.GetFullPath(Path.Combine(ExternalImagesPath, folder, fileName));

        if (!File.Exists(fullPath))
        {
            return null;
        }

        return new Uri(fullPath, UriKind.Absolute);
    }

    private static Uri ToPackUri(string relativePath)
    {
        var cleaned = relativePath?.Replace("\\", "/").TrimStart('/');
        return new Uri($"pack://application:,,,/{cleaned}", UriKind.Absolute);
    }
}