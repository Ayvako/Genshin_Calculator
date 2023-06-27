using Genshin.src.LevelingResources;
using System.Collections.Generic;
using System.Linq;

namespace Genshin.src.Upgrades
{
    public class SkillUpgrade
    {
        public static List<Material> GetCost(Character character, int from, int to)
        {
            var materials = Enumerable.Range(from + 1, to - from).SelectMany(i => GetMaterials(character)[i]);

            var groupedMaterials = materials.GroupBy(m => m.Name).Select(g => new Material(g.Key, g.First().Type, g.First().Rarity, g.Sum(m => m.Amount))).ToList();



            return groupedMaterials;
        }

        private static Dictionary<int, Material[]> GetMaterials(Character сharacter)
        {
            return new(){
                {2,new Material[]{ new($"{Book.GetMaterial(сharacter,"green")}", MaterailTypes.BOOK, 2, 3), new ($"{Enemy.GetMaterial(сharacter, "white")}", MaterailTypes.ENEMY, 1, 6), new ("Mora", MaterailTypes.OTHER, 1, 12500) } },

                {3,new Material[]{ new ($"{Book.GetMaterial(сharacter, "blue")}", MaterailTypes.BOOK, 3, 2),  new ($"{Enemy.GetMaterial(сharacter, "green")}", MaterailTypes.ENEMY, 2, 3), new ("Mora", MaterailTypes.OTHER, 1, 17500) } },
                {4,new Material[]{ new ($"{Book.GetMaterial(сharacter, "blue")}", MaterailTypes.BOOK, 3, 4),  new ($"{Enemy.GetMaterial(сharacter, "green")}", MaterailTypes.ENEMY, 2, 4), new ("Mora", MaterailTypes.OTHER, 1, 25000) } },
                {5,new Material[]{ new ($"{Book.GetMaterial(сharacter, "blue")}", MaterailTypes.BOOK, 3, 6),  new ($"{Enemy.GetMaterial(сharacter, "green")}", MaterailTypes.ENEMY, 2, 6), new ("Mora", MaterailTypes.OTHER, 1, 30000) } },
                {6,new Material[]{ new ($"{Book.GetMaterial(сharacter, "blue")}", MaterailTypes.BOOK, 3, 9),  new ($"{Enemy.GetMaterial(сharacter, "green")}", MaterailTypes.ENEMY, 2, 9), new ("Mora", MaterailTypes.OTHER, 1, 37500) } },

                {7,new Material[]{ new ($"{Book.GetMaterial(сharacter, "violet")}", MaterailTypes.BOOK, 4, 4),   new ($"{Enemy.GetMaterial(сharacter, "blue")}", MaterailTypes.ENEMY, 3, 4), new ($"{сharacter.Assets.WeeklyBoss}", MaterailTypes.OTHER, 5, 1), new ("Mora", MaterailTypes.OTHER, 1, 120000) } },
                {8,new Material[]{ new ($"{Book.GetMaterial(сharacter, "violet")}", MaterailTypes.BOOK, 4, 6),   new ($"{Enemy.GetMaterial(сharacter, "blue")}", MaterailTypes.ENEMY, 3, 6), new ($"{сharacter.Assets.WeeklyBoss}", MaterailTypes.OTHER, 5, 1), new ("Mora", MaterailTypes.OTHER, 1, 260000) } },
                {9,new Material[]{ new ($"{Book.GetMaterial(сharacter, "violet")}", MaterailTypes.BOOK, 4, 12),  new ($"{Enemy.GetMaterial(сharacter, "blue")}", MaterailTypes.ENEMY, 3, 9), new ($"{сharacter.Assets.WeeklyBoss}", MaterailTypes.OTHER, 5, 2), new ("Mora", MaterailTypes.OTHER, 1, 450000) } },
                {10,new Material[]{new ($"{Book.GetMaterial(сharacter, "violet")}", MaterailTypes.BOOK, 4, 16),  new ($"{Enemy.GetMaterial(сharacter, "blue")}", MaterailTypes.ENEMY, 3, 12),new ($"{сharacter.Assets.WeeklyBoss}", MaterailTypes.OTHER, 5, 2), new ("Mora", MaterailTypes.OTHER, 1, 700000) , new ("CrownOfInsight", MaterailTypes.OTHER, 5, 1) } },
            };
        }
    }
}