using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Services.MaterialProviders;

public static class MaterialGenerator
{
    public static List<Material> GenerateDynamicMaterials(List<Character> characters)
    {
        var materials = new List<Material>();

        var skillBaseNames = characters.Select(c => c.Assets!.SkillMaterials).Distinct().Where(n => !string.IsNullOrEmpty(n));
        foreach (var baseName in skillBaseNames)
        {
            materials.Add(new Material($"TeachingsOf{baseName}", MaterialTypes.SkillMaterial, MaterialRarity.Green, 0));
            materials.Add(new Material($"GuideTo{baseName}", MaterialTypes.SkillMaterial, MaterialRarity.Blue, 0));
            materials.Add(new Material($"PhilosophiesOf{baseName}", MaterialTypes.SkillMaterial, MaterialRarity.Violet, 0));
        }

        var elements = characters.Select(c => c.Assets!.Element).Distinct();
        foreach (var element in elements)
        {
            string baseName = GemMaterialProvider.GetBaseGemName(element);
            materials.Add(new Material($"{baseName}Sliver", MaterialTypes.Gem, MaterialRarity.Green, 0));
            materials.Add(new Material($"{baseName}Fragment", MaterialTypes.Gem, MaterialRarity.Blue, 0));
            materials.Add(new Material($"{baseName}Chunk", MaterialTypes.Gem, MaterialRarity.Violet, 0));
            materials.Add(new Material($"{baseName}Gemstone", MaterialTypes.Gem, MaterialRarity.Orange, 0));
        }

        AddUnique(materials, characters, c => c.Assets!.LocalSpecialty, MaterialTypes.LocalSpecialty, MaterialRarity.White);
        AddUnique(materials, characters, c => c.Assets!.MiniBoss, MaterialTypes.MiniBoss, MaterialRarity.Violet);
        AddUnique(materials, characters, c => c.Assets!.WeeklyBoss, MaterialTypes.WeeklyBoss, MaterialRarity.Orange);

        materials.Add(new Material("CrownOfInsight", MaterialTypes.Other, MaterialRarity.Blue, 0));
        materials.Add(new Material("Mora", MaterialTypes.Mora, MaterialRarity.Blue, 0));
        materials.Add(new Material("WanderersAdvice", MaterialTypes.Exp, MaterialRarity.Green, 0));
        materials.Add(new Material("AdventurersExperience", MaterialTypes.Exp, MaterialRarity.Blue, 0));
        materials.Add(new Material("HerosWit", MaterialTypes.Exp, MaterialRarity.Violet, 0));

        return materials;
    }

    private static void AddUnique(List<Material> list, IEnumerable<Character> chars, Func<Character, string> selector, MaterialTypes type, MaterialRarity rarity)
    {
        var names = chars.Select(selector).Distinct().Where(n => !string.IsNullOrEmpty(n));
        foreach (var name in names)
        {
            list.Add(new Material(name, type, rarity, 0));
        }
    }
}