using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.MaterialProviders;

public sealed class GemMaterialProvider : IMaterialProvider
{
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

    public string GetMaterial(Character character, MaterialRarity rarity)
    {
        string baseName = GetBaseGemName(character.Assets!.Element);

        return rarity switch
        {
            MaterialRarity.Green => $"{baseName}Sliver",
            MaterialRarity.Blue => $"{baseName}Fragment",
            MaterialRarity.Violet => $"{baseName}Chunk",
            MaterialRarity.Orange => $"{baseName}Gemstone",
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), "Gems only have Green, Blue, Violet, and Orange rarities"),
        };
    }

    public IEnumerable<string> GetMaterialGroup(Character character)
    {
        string baseName = GetBaseGemName(character.Assets!.Element);
        return [
            $"{baseName}Sliver",
            $"{baseName}Fragment",
            $"{baseName}Chunk",
            $"{baseName}Gemstone"
        ];
    }
}