using System;
using System.Collections.Generic;
using System.Linq;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.LevelingResources;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services
{
    public static class SkillUpgradeService
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

        public static List<Material> GetSkillsCost(Character character)
        {
            Skill auto_attack = character.AutoAttack;
            Skill elemental = character.Elemental;
            Skill burst = character.Burst;

            List<Material> materialsForAA = GetCost(character, auto_attack.CurrentLevel, auto_attack.DesiredLevel);
            List<Material> materialsForElem = GetCost(character, elemental.CurrentLevel, elemental.DesiredLevel);
            List<Material> materialsForBurst = GetCost(character, burst.CurrentLevel, burst.DesiredLevel);

            return InventoryUtils.Merge(materialsForAA, materialsForElem, materialsForBurst);
        }

        public static List<Material> GetCost(Character character, int from, int to)
        {
            var materials = Enumerable.Range(from + 1, to - from).SelectMany(i => GetMaterials(character)[i]);

            var groupedMaterials = materials.GroupBy(m => m.Name).Select(g => new Material(g.Key, g.First().Type, g.First().Rarity, g.Sum(m => m.Amount))).ToList();

            return groupedMaterials;
        }

        private static Dictionary<int, Material[]> GetMaterials(Character character)
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
                            Book.GetMaterial(character, cfg.BookRarity) ?? throw new InvalidOperationException("Book material is null"),
                            MaterailTypes.Book,
                            cfg.BookRarity,
                            cfg.BookAmount),

                        new(
                            Enemy.GetMaterial(character, cfg.EnemyRarity) ?? throw new InvalidOperationException("Enemy material is null"),
                            MaterailTypes.Enemy,
                            cfg.EnemyRarity,
                            cfg.EnemyAmount),

                        new("Mora", MaterailTypes.Other, MaterialRarity.Blue, cfg.MoraAmount),
                    };

                    if (cfg.WeeklyBossAmount > 0)
                    {
                        materials.Add(new(weeklyBoss, MaterailTypes.Other, MaterialRarity.Orange, cfg.WeeklyBossAmount));
                    }

                    if (cfg.CrownAmount > 0)
                    {
                        materials.Add(new("CrownOfInsight", MaterailTypes.Other, MaterialRarity.Orange, cfg.CrownAmount));
                    }

                    return new { level, materials = materials.ToArray() };
                })
                .ToDictionary(x => x.level, x => x.materials);
        }
    }
}