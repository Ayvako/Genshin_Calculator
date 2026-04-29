using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public static class MaterialGenerator
{
    public static List<Material> GenerateDynamicMaterials(List<Character> characters)
    {
        var materials = new List<Material>();

        var skillMaterials = characters
            .Select(c => c.Assets!.SkillMaterials)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .SelectMany(baseName =>
            {
                var names = MaterialNaming.GetSkillNames(baseName);
                return new[]
                {
                    new Material(names[0], MaterialTypes.SkillMaterial, MaterialRarity.Green, 0),
                    new Material(names[1], MaterialTypes.SkillMaterial, MaterialRarity.Blue, 0),
                    new Material(names[2], MaterialTypes.SkillMaterial, MaterialRarity.Violet, 0),
                };
            });

        materials.AddRange(skillMaterials);

        var gemMaterials = characters
            .Select(c => c.Assets!.Element)
            .Distinct()
            .SelectMany(element =>
            {
                var names = MaterialNaming.GetGemNames(element);
                return new[]
                {
                    new Material(names[0], MaterialTypes.Gem, MaterialRarity.Green, 0),
                    new Material(names[1], MaterialTypes.Gem, MaterialRarity.Blue, 0),
                    new Material(names[2], MaterialTypes.Gem, MaterialRarity.Violet, 0),
                    new Material(names[3], MaterialTypes.Gem, MaterialRarity.Orange, 0),
                };
            });

        materials.AddRange(gemMaterials);

        AddUnique(materials, characters, c => c.Assets!.LocalSpecialty, MaterialTypes.LocalSpecialty, MaterialRarity.White);
        AddUnique(materials, characters, c => c.Assets!.MiniBoss, MaterialTypes.MiniBoss, MaterialRarity.Violet);
        AddUnique(materials, characters, c => c.Assets!.WeeklyBoss, MaterialTypes.WeeklyBoss, MaterialRarity.Orange);

        materials.AddRange(
        [
            new Material("StellaFortuna", MaterialTypes.StellaFortuna, MaterialRarity.Orange, 0),
            new Material("CrownOfInsight", MaterialTypes.Crown, MaterialRarity.Orange, 0),
            new Material("Mora", MaterialTypes.Mora, MaterialRarity.Blue, 0),
            new Material("WanderersAdvice", MaterialTypes.Exp, MaterialRarity.Green, 0),
            new Material("AdventurersExperience", MaterialTypes.Exp, MaterialRarity.Blue, 0),
            new Material("HerosWit", MaterialTypes.Exp, MaterialRarity.Violet, 0),
        ]);

        return materials;
    }

    private static void AddUnique(List<Material> list, IEnumerable<Character> chars, Func<Character, string> selector, MaterialTypes type, MaterialRarity rarity)
    {
        var names = chars.Select(selector).Where(n => !string.IsNullOrEmpty(n)).Distinct();
        list.AddRange(names.Select(name => new Material(name, type, rarity, 0)));
    }
}