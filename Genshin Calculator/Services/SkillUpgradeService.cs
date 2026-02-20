using System;
using System.Collections.Generic;
using System.Linq;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.Interfaces;
using Genshin_Calculator.Services.Materials;

namespace Genshin_Calculator.Services;

public class SkillUpgradeService : ISkillUpgradeService
{
    private static readonly LevelMaterialData[] LevelConfigs =
    [
        new(MaterialRarity.Green, MaterialRarity.White, 3, 6, 12500),
        new(MaterialRarity.Blue, MaterialRarity.Green, 2, 3, 17500),
        new(MaterialRarity.Blue, MaterialRarity.Green, 4, 4, 25000),
        new(MaterialRarity.Blue, MaterialRarity.Green, 6, 6, 30000),
        new(MaterialRarity.Blue, MaterialRarity.Green, 9, 9, 37500),
        new(MaterialRarity.Violet, MaterialRarity.Blue, 4, 4, 120000, 1),
        new(MaterialRarity.Violet, MaterialRarity.Blue, 6, 6, 260000, 1),
        new(MaterialRarity.Violet, MaterialRarity.Blue, 12, 9, 450000, 2),
        new(MaterialRarity.Violet, MaterialRarity.Blue, 16, 12, 700000, 2, 1),
    ];

    private readonly SkillMaterialProvider skillMaterials;

    private readonly EnemyMaterialProvider enemies;

    public SkillUpgradeService(
        SkillMaterialProvider skillMaterials,
        EnemyMaterialProvider enemies)
    {
        this.skillMaterials = skillMaterials;
        this.enemies = enemies;
    }

    public List<Material> GetSkillsCost(Character character)
    {
        Skill auto_attack = character.AutoAttack;
        Skill elemental = character.Elemental;
        Skill burst = character.Burst;

        List<Material> materialsForAA = this.GetCost(character, auto_attack.CurrentLevel, auto_attack.DesiredLevel);
        List<Material> materialsForElem = this.GetCost(character, elemental.CurrentLevel, elemental.DesiredLevel);
        List<Material> materialsForBurst = this.GetCost(character, burst.CurrentLevel, burst.DesiredLevel);

        return Merge(materialsForAA, materialsForElem, materialsForBurst);
    }

    private static List<Material> Merge(params List<Material>[] dictionaries)
    {
        IEnumerable<Material> merged = dictionaries[0];
        for (int i = 1; i < dictionaries.Length; i++)
        {
            merged = merged.Concat(dictionaries[i]);
        }

        var groupedMaterials = merged.GroupBy(m => new { m.Name })
            .Select(g => new Material(g.Key.Name, g.First().Type, g.First().Rarity, g.Sum(m => m.Amount)))
            .ToList();

        return groupedMaterials;
    }

    private List<Material> GetCost(Character character, int from, int to)
    {
        var materials = Enumerable.Range(from + 1, to - from).SelectMany(i => this.GetMaterials(character)[i]);

        var groupedMaterials = materials.GroupBy(m => m.Name).Select(g => new Material(g.Key, g.First().Type, g.First().Rarity, g.Sum(m => m.Amount))).ToList();

        return groupedMaterials;
    }

    private Dictionary<int, Material[]> GetMaterials(Character character)
    {
        ArgumentNullException.ThrowIfNull(character);
        ArgumentNullException.ThrowIfNull(character.Assets);

        string weeklyBoss = character.Assets.WeeklyBoss ?? throw new InvalidOperationException("WeeklyBoss cannot be null");

        return LevelConfigs
            .Select((cfg, idx) =>
            {
                int level = idx + 2;
                var materials = new List<Material>
                {
                    new(
                        this.skillMaterials.GetMaterial(character, cfg.SkillMaterialRarity) ?? throw new InvalidOperationException("SkillMaterial material is null"),
                        MaterialTypes.SkillMaterial,
                        cfg.SkillMaterialRarity,
                        cfg.SkillMaterialAmount),

                    new(
                        this.enemies.GetMaterial(character, cfg.EnemyRarity) ?? throw new InvalidOperationException("Enemy material is null"),
                        MaterialTypes.Enemy,
                        cfg.EnemyRarity,
                        cfg.EnemyAmount),

                    new("Mora", MaterialTypes.Other, MaterialRarity.Blue, cfg.MoraAmount),
                };

                if (cfg.WeeklyBossAmount > 0)
                {
                    materials.Add(new(weeklyBoss, MaterialTypes.Other, MaterialRarity.Orange, cfg.WeeklyBossAmount));
                }

                if (cfg.CrownAmount > 0)
                {
                    materials.Add(new("CrownOfInsight", MaterialTypes.Other, MaterialRarity.Orange, cfg.CrownAmount));
                }

                return new { level, materials = materials.ToArray() };
            })
            .ToDictionary(x => x.level, x => x.materials);
    }
}