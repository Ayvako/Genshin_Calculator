using System;

namespace Genshin_Calculator.Helpers;

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

    private static object Elements => Combine(BasePath, "Elements");

    public static Uri Character(string name) => ToPackUri($"{Characters}/{name}.png");

    public static Uri Material(string name) => ToPackUri($"{Materials}/{name}.png");

    public static Uri Tool(string name) => ToPackUri($"{Tools}/{name}.png");

    public static Uri Element(string name) => ToPackUri($"{Elements}/{name}.png");

    public static Uri Weapon(string name) => ToPackUri($"{Weapons}/{name}.png");

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