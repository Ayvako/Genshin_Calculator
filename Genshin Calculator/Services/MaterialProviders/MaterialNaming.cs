using Genshin_Calculator.Core.Models.Enums;
using System;

namespace Genshin_Calculator.Services.MaterialProviders;

public static class MaterialNaming
{
    public static string[] GetSkillNames(string baseName) =>
        [$"TeachingsOf{baseName}", $"GuideTo{baseName}", $"PhilosophiesOf{baseName}"];

    public static string GetBaseGemName(Element element) => element switch
    {
        Element.Cryo => "ShivadaJade",
        Element.Electro => "VajradaAmethyst",
        Element.Dendro => "NagadusEmerald",
        Element.Pyro => "AgnidusAgate",
        Element.Geo => "PrithivaTopaz",
        Element.Hydro => "VarunadaLazurite",
        Element.Anemo => "VayudaTurquoise",
        _ => throw new ArgumentException($"Unknown element: {element}"),
    };

    public static string[] GetGemNames(Element element)
    {
        var @base = GetBaseGemName(element);
        return [$"{@base}Sliver", $"{@base}Fragment", $"{@base}Chunk", $"{@base}Gemstone"];
    }
}