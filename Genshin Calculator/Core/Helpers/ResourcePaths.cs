using System;
using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Core.Helpers;

public static class ResourcePaths
{
    private static string basePath = "Resources/Images";

    public static string BasePath
    {
        get => basePath;
        set => basePath = (value ?? string.Empty).TrimEnd('/', '\\');
    }

    public static Uri Character(string name) => ToPackUri($"{BasePath}/Characters/{name}.png");

    public static Uri Material(string name) => ToPackUri($"{BasePath}/Materials/{name}.png");

    public static Uri Tool(string name) => ToPackUri($"{BasePath}/Tools/{name}.png");

    public static Uri Element(Element name) => ToPackUri($"{BasePath}/Elements/{name}.png");

    public static Uri Weapon(WeaponType name) => ToPackUri($"{BasePath}/Weapons/{name}.png");

    public static Uri Star(MaterialRarity name) => ToPackUri($"{BasePath}/Stars/{name}.png");

    public static string MaterialsJson(string name)
    {
        return $"Genshin_Calculator.Resources.Json.{name}.json";
    }

    public static string ToEmbeddedResource(string relativePath)
    {
        return "Genshin_Calculator." + relativePath.Replace("/", ".").Replace("\\", ".");
    }

    private static Uri ToPackUri(string relativePath)
    {
        var cleaned = relativePath?.Replace("\\", "/").TrimStart('/');
        return new Uri($"pack://application:,,,/{cleaned}", UriKind.Absolute);
    }
}