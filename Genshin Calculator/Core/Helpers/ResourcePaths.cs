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

    private static string Characters => Combine(BasePath, "Characters");

    private static string Materials => Combine(BasePath, "Materials");

    private static string Tools => Combine(BasePath, "Tools");

    private static string Weapons => Combine(BasePath, "Weapons");

    private static string Elements => Combine(BasePath, "Elements");

    private static string Stars => Combine(BasePath, "Stars");

    public static Uri Character(string name) => ToPackUri($"{Characters}/{name}.png");

    public static Uri Material(string name) => ToPackUri($"{Materials}/{name}.png");

    public static Uri Tool(string name) => ToPackUri($"{Tools}/{name}.png");

    public static Uri Element(Element name) => ToPackUri($"{Elements}/{name}.png");

    public static Uri Weapon(WeaponType name) => ToPackUri($"{Weapons}/{name}.png");

    public static Uri Star(MaterialRarity name) => ToPackUri($"{Stars}/{name}.png");

    public static Uri MaterialsJson(string name) => ToPackUri($"Resources/Json/{name}.json");

    private static Uri ToPackUri(string relativePath)
    {
        var cleaned = relativePath?.Replace("\\", "/").TrimStart('/');
        var pack = $"pack://application:,,,/{cleaned}";
        return new Uri(pack, UriKind.Absolute);
    }

    private static string Combine(string a, string b)
    {
        if (string.IsNullOrWhiteSpace(a))
        {
            return b ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(b))
        {
            return a;
        }

        return $"{a.TrimEnd('/', '\\')}/{b.TrimStart('/', '\\')}";
    }
}