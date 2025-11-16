using System;
using System.Collections.Generic;
using System.Linq;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.LevelingResources;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services
{
    public static class CharacterUpgradeService
    {
        private static readonly List<string> Levels = [
            "1",  "2",  "3",  "4", "5", "6", "7", "8", "9", "10",
            "11",  "12", "13",  "14", "15", "16", "17", "18", "19", "20", "20+",
            "21",  "22", "23",  "24", "25", "26", "27", "28", "29", "30",
            "31",  "32", "33",  "34", "35", "36", "37", "38", "39", "40", "40+",
            "41",  "42", "43",  "44", "45", "46", "47", "48", "49", "50", "50+",
            "51",  "52", "53",  "54", "55", "56", "57", "58", "59", "60", "60+",
            "61",  "62", "63",  "64", "65", "66", "67", "68", "69", "70", "70+",
            "71",  "72", "73",  "74", "75", "76", "77", "78", "79", "80", "80+",
            "81",  "82", "83",  "84", "85", "86", "87", "88", "89", "90"
        ];

        public static List<Material> GetCharacterCost(Character character)
        {
            return GetCost(character, character.CurrentLevel, character.DesiredLevel);
        }

        public static List<Material> GetCost(Character character, string from, string to)
        {
            List<Material> requiredMaterials = [];
            Dictionary<string, Material[]> amountMaterials = GetMaterials(character);

            int startIndex = Levels.FindIndex(s => s.Contains(from));
            int endIndex = Levels.FindIndex(s => s.Contains(to));

            var materialsInRange = Levels.Skip(startIndex + 1).Take(endIndex - startIndex);

            foreach (var material in materialsInRange.SelectMany(level => amountMaterials[level]))
            {
                var existingMaterial = requiredMaterials.FirstOrDefault(m => m.Name == material.Name);

                if (existingMaterial != null)
                {
                    existingMaterial.Amount += material.Amount;
                }
                else
                {
                    requiredMaterials.Add(new Material(material.Name, material.Type, material.Rarity, material.Amount));
                }
            }

            return requiredMaterials;
        }

        private static Dictionary<string, Material[]> GetMaterials(Character character)
        {
            ArgumentNullException.ThrowIfNull(character);

            ArgumentNullException.ThrowIfNull(character.Assets);

            return new()
            {
                { "2", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 1), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 200) } },
                { "3", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 1325 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 265) } },
                { "4", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 1700 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 1700 / 5) } },
                { "5", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 2150 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 2150 / 5) } },
                { "6", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 2625 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 2625 / 5) } },
                { "7", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 3150 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 3150 / 5) } },
                { "8", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 3725 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 3725 / 5) } },
                { "9", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 4350 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 4350 / 5) } },
                { "10", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 5000 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 5000 / 5) } },
                { "11", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 5700 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 5700 / 5) } },
                { "12", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 6450 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 6450 / 5) } },
                { "13", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 7225 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 7225 / 5) } },
                { "14", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 8050 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 8050 / 5) } },
                { "15", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 8925 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 8925 / 5) } },
                { "16", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 9825 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 9825 / 5) } },
                { "17", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 10750 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 10750 / 5) } },
                { "18", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 11725 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 11725 / 5) } },
                { "19", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 12725 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 12725 / 5) } },
                { "20", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 13775 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 13775 / 5) } },
                { "21", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 14875 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 14875 / 5) } },
                { "22", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 16800 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 16800 / 5) } },
                { "23", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 18000 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 18000 / 5) } },
                { "24", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 19250 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 19250 / 5) } },
                { "25", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 20550 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 20550 / 5) } },
                { "26", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 21875 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 21875 / 5) } },
                { "27", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 23250 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 23250 / 5) } },
                { "28", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 24650 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 24650 / 5) } },
                { "29", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 26100 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 26100 / 5) } },
                { "30", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 27575 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 27575 / 5) } },
                { "31", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 29100 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 29100 / 5) } },
                { "32", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 30650 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 30650 / 5) } },
                { "33", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 32250 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 32250 / 5) } },
                { "34", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 33875 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 33875 / 5) } },
                { "35", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 35550 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 35550 / 5) } },
                { "36", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 37250 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 37250 / 5) } },
                { "37", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 38975 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 38975 / 5) } },
                { "38", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 40750 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 40750 / 5) } },
                { "39", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 42575 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 42575 / 5) } },
                { "40", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 44425 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 44425 / 5) } },
                { "41", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 46300 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 50625 / 5) } },
                { "42", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 50625 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 50625 / 5) } },
                { "43", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 52700 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 52700 / 5) } },
                { "44", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 54775 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 54775 / 5) } },
                { "45", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 56900 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 56900 / 5) } },
                { "46", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 59075 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 59075 / 5) } },
                { "47", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 61275 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 61275 / 5) } },
                { "48", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 63525 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 63525 / 5) } },
                { "49", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 65800 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 65800 / 5) } },
                { "50", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 68125 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 68125 / 5) } },
                { "51", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 70475 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 70475 / 5) } },
                { "52", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 76500 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 76500 / 5) } },
                { "53", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 79050 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 79050 / 5) } },
                { "54", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 81650 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 81650 / 5) } },
                { "55", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 84275 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 84275 / 5) } },
                { "56", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 86950 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 86950 / 5) } },
                { "57", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 89650 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 89650 / 5) } },
                { "58", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 92400 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 92400 / 5) } },
                { "59", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 95175 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 95175 / 5) } },
                { "60", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 98000 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 98000 / 5) } },
                { "61", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 100875 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 100875 / 5) } },
                { "62", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 108950 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 108950 / 5) } },
                { "63", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 112050 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 112050 / 5) } },
                { "64", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 115175 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 115175 / 5) } },
                { "65", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 118325 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 118325 / 5) } },
                { "66", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 121525 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 121525 / 5) } },
                { "67", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 124775 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 124775 / 5) } },
                { "68", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 128075 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 128075 / 5) } },
                { "69", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 131400 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 131400 / 5) } },
                { "70", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 134775 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 134775 / 5) } },
                { "71", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 138175 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 138175 / 5) } },
                { "72", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 148700 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 148700 / 5) } },
                { "73", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 152375 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 152375 / 5) } },
                { "74", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 156075 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 156075 / 5) } },
                { "75", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 159825 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 159825 / 5) } },
                { "76", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 163600 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 163600 / 5) } },
                { "77", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 167425 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 167425 / 5) } },
                { "78", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 171300 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 171300 / 5) } },
                { "79", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 175225 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 175225 / 5) } },
                { "80", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 179175 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 179175 / 5) } },
                { "81", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 183175 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 183175 / 5) } },
                { "82", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 216225 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 216225 / 5) } },
                { "83", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 243025 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 243025 / 5) } },
                { "84", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 273100 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 273100 / 5) } },
                { "85", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 306800 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 306800 / 5) } },
                { "86", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 344600 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 344600 / 5) } },
                { "87", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 386950 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 386950 / 5) } },
                { "88", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 434425 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 434425 / 5) } },
                { "89", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 487625 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 487625 / 5) } },
                { "90", new Material[] { new ("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 547200 / 1000), new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 547200 / 5) } },
                { "20+", new Material[] { new ($"{Gem.GetMaterial(character, MaterialRarity.Green)}",  MaterailTypes.Gem, MaterialRarity.Green, 1),   new ($"{character.Assets!.LocalSpecialty}", MaterailTypes.Other, MaterialRarity.White, 3),     new ($"{Enemy.GetMaterial(character, MaterialRarity.White)}", MaterailTypes.Enemy, MaterialRarity.White, 3),    new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 20000) } },
                { "40+", new Material[] { new ($"{Gem.GetMaterial(character, MaterialRarity.Blue)}",   MaterailTypes.Gem, MaterialRarity.Blue, 3),    new ($"{character.Assets!.LocalSpecialty}", MaterailTypes.Other, MaterialRarity.White, 10),  new ($"{character.Assets.MiniBoss}", MaterailTypes.Other, MaterialRarity.Violet, 2),  new ($"{Enemy.GetMaterial(character, MaterialRarity.White)}", MaterailTypes.Enemy, MaterialRarity.White, 15),   new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 40000) } },
                { "50+", new Material[] { new ($"{Gem.GetMaterial(character, MaterialRarity.Blue)}",   MaterailTypes.Gem, MaterialRarity.Blue, 6),    new ($"{character.Assets!.LocalSpecialty}", MaterailTypes.Other, MaterialRarity.White, 20),  new ($"{character.Assets.MiniBoss}", MaterailTypes.Other, MaterialRarity.Violet, 4),  new ($"{Enemy.GetMaterial(character, MaterialRarity.Green)}", MaterailTypes.Enemy, MaterialRarity.Green, 12),   new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 60000) } },
                { "60+", new Material[] { new ($"{Gem.GetMaterial(character, MaterialRarity.Violet)}", MaterailTypes.Gem, MaterialRarity.Violet, 3),  new ($"{character.Assets!.LocalSpecialty}", MaterailTypes.Other, MaterialRarity.White, 30),  new ($"{character.Assets.MiniBoss}", MaterailTypes.Other, MaterialRarity.Violet, 8),  new ($"{Enemy.GetMaterial(character, MaterialRarity.Green)}", MaterailTypes.Enemy, MaterialRarity.Green, 18),   new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 80000) } },
                { "70+", new Material[] { new ($"{Gem.GetMaterial(character, MaterialRarity.Violet)}", MaterailTypes.Gem, MaterialRarity.Violet, 6),  new ($"{character.Assets!.LocalSpecialty}", MaterailTypes.Other, MaterialRarity.White, 45),  new ($"{character.Assets.MiniBoss}", MaterailTypes.Other, MaterialRarity.Violet, 12),  new ($"{Enemy.GetMaterial(character, MaterialRarity.Blue)}",  MaterailTypes.Enemy, MaterialRarity.Blue, 12),   new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 100000) } },
                { "80+", new Material[] { new ($"{Gem.GetMaterial(character, MaterialRarity.Orange)}", MaterailTypes.Gem, MaterialRarity.Orange, 6),  new ($"{character.Assets!.LocalSpecialty}", MaterailTypes.Other, MaterialRarity.White, 60),  new ($"{character.Assets.MiniBoss}", MaterailTypes.Other, MaterialRarity.Violet, 20),  new ($"{Enemy.GetMaterial(character, MaterialRarity.Blue)}",  MaterailTypes.Enemy, MaterialRarity.Blue, 24),   new ("Mora", MaterailTypes.Other, MaterialRarity.Blue, 120000) } },
            };
        }
    }
}