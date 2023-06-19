using Genshin.src.LevelingResources;
using System.Collections.Generic;
using System.Linq;

namespace Genshin.src.Upgrades
{
    public class CharacterUpgrade
    {
        public static List<Material> GetCost(Character character, string from, string to)
        {
            List<Material> RequiredMaterials = new();
            Dictionary<string, Material[]> AmountMaterials = GetMaterials(character);

            int startIndex = Levels.FindIndex(s => s.Contains(from));
            int endIndex = Levels.FindIndex(s => s.Contains(to));

            var materialsInRange = Levels.Skip(startIndex + 1).Take(endIndex - startIndex);

            foreach (var material in materialsInRange.SelectMany(level => AmountMaterials[level]))
            {
                var existingMaterial = RequiredMaterials.FirstOrDefault(m => m.Name == material.Name);

                if (existingMaterial != null)
                {
                    existingMaterial.Amount += material.Amount;
                }
                else
                {
                    RequiredMaterials.Add(new Material(material.Name, material.Type, material.Amount));

                }
            }

            return RequiredMaterials;
        }
        private static Dictionary<string, Material[]> GetMaterials(Character character)
        {
            return new()
            {
                { "2", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 1000 / 1000), new Material("Mora", MaterailTypes.OTHER, 200) } },
                { "3", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 1325 / 1000), new Material("Mora", MaterailTypes.OTHER, 265) } },
                { "4", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 1700 / 1000), new Material("Mora", MaterailTypes.OTHER, 1700 / 5) } },
                { "5", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 2150 / 1000), new Material("Mora", MaterailTypes.OTHER, 2150 / 5) } },
                { "6", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 2625 / 1000), new Material("Mora", MaterailTypes.OTHER, 2625 / 5) } },
                { "7", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 3150 / 1000), new Material("Mora", MaterailTypes.OTHER, 3150 / 5) } },
                { "8", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 3725 / 1000), new Material("Mora", MaterailTypes.OTHER, 3725 / 5) } },
                { "9", new Material[]  { new Material("WanderersAdvice", MaterailTypes.EXP, 4350 / 1000), new Material("Mora", MaterailTypes.OTHER, 4350 / 5) } },
                { "10", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 5000 / 1000), new Material("Mora", MaterailTypes.OTHER, 5000 / 5) } },
                { "11", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 5700 / 1000), new Material("Mora", MaterailTypes.OTHER, 5700 / 5) } },
                { "12", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 6450 / 1000), new Material("Mora", MaterailTypes.OTHER, 6450 / 5) } },
                { "13", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 7225 / 1000), new Material("Mora", MaterailTypes.OTHER, 7225 / 5) } },
                { "14", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 8050 / 1000), new Material("Mora", MaterailTypes.OTHER, 8050 / 5) } },
                { "15", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 8925 / 1000), new Material("Mora", MaterailTypes.OTHER, 8925 / 5) } },
                { "16", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 9825 / 1000), new Material("Mora", MaterailTypes.OTHER, 9825 / 5) } },
                { "17", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 10750 / 1000), new Material("Mora", MaterailTypes.OTHER, 10750 / 5) } },
                { "18", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 11725 / 1000), new Material("Mora", MaterailTypes.OTHER, 11725 / 5) } },
                { "19", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 12725 / 1000), new Material("Mora", MaterailTypes.OTHER, 12725 / 5) } },
                { "20", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 13775 / 1000), new Material("Mora", MaterailTypes.OTHER, 13775 / 5) } },
                { "21", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 14875 / 1000), new Material("Mora", MaterailTypes.OTHER, 14875 / 5) } },
                { "22", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 16800 / 1000), new Material("Mora", MaterailTypes.OTHER, 16800 / 5) } },
                { "23", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 18000 / 1000), new Material("Mora", MaterailTypes.OTHER, 18000 / 5) } },
                { "24", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 19250 / 1000), new Material("Mora", MaterailTypes.OTHER, 19250 / 5) } },
                { "25", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 20550 / 1000), new Material("Mora", MaterailTypes.OTHER, 20550 / 5) } },
                { "26", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 21875 / 1000), new Material("Mora", MaterailTypes.OTHER, 21875 / 5) } },
                { "27", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 23250 / 1000), new Material("Mora", MaterailTypes.OTHER, 23250 / 5) } },
                { "28", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 24650 / 1000), new Material("Mora", MaterailTypes.OTHER, 24650 / 5) } },
                { "29", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 26100 / 1000), new Material("Mora", MaterailTypes.OTHER, 26100 / 5) } },
                { "30", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 27575 / 1000), new Material("Mora", MaterailTypes.OTHER, 27575 / 5) } },
                { "31", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 29100 / 1000), new Material("Mora", MaterailTypes.OTHER, 29100 / 5) } },
                { "32", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 30650 / 1000), new Material("Mora", MaterailTypes.OTHER, 30650 / 5) } },
                { "33", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 32250 / 1000), new Material("Mora", MaterailTypes.OTHER, 32250 / 5) } },
                { "34", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 33875 / 1000), new Material("Mora", MaterailTypes.OTHER, 33875 / 5) } },
                { "35", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 35550 / 1000), new Material("Mora", MaterailTypes.OTHER, 35550 / 5) } },
                { "36", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 37250 / 1000), new Material("Mora", MaterailTypes.OTHER, 37250 / 5) } },
                { "37", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 38975 / 1000), new Material("Mora", MaterailTypes.OTHER, 38975 / 5) } },
                { "38", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 40750 / 1000), new Material("Mora", MaterailTypes.OTHER, 40750 / 5) } },
                { "39", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 42575 / 1000), new Material("Mora", MaterailTypes.OTHER, 42575 / 5) } },
                { "40", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 44425 / 1000), new Material("Mora", MaterailTypes.OTHER, 44425 / 5) } },
                { "41", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 46300 / 1000), new Material("Mora", MaterailTypes.OTHER, 50625 / 5) } },
                { "42", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 50625 / 1000), new Material("Mora", MaterailTypes.OTHER, 50625 / 5) } },
                { "43", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 52700 / 1000), new Material("Mora", MaterailTypes.OTHER, 52700 / 5) } },
                { "44", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 54775 / 1000), new Material("Mora", MaterailTypes.OTHER, 54775 / 5) } },
                { "45", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 56900 / 1000), new Material("Mora", MaterailTypes.OTHER, 56900 / 5) } },
                { "46", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 59075 / 1000), new Material("Mora", MaterailTypes.OTHER, 59075 / 5) } },
                { "47", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 61275 / 1000), new Material("Mora", MaterailTypes.OTHER, 61275 / 5) } },
                { "48", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 63525 / 1000), new Material("Mora", MaterailTypes.OTHER, 63525 / 5) } },
                { "49", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 65800 / 1000), new Material("Mora", MaterailTypes.OTHER, 65800 / 5) } },
                { "50", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 68125 / 1000), new Material("Mora", MaterailTypes.OTHER, 68125 / 5) } },
                { "51", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 70475 / 1000), new Material("Mora", MaterailTypes.OTHER, 70475 / 5) } },
                { "52", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 76500 / 1000), new Material("Mora", MaterailTypes.OTHER, 76500 / 5) } },
                { "53", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 79050 / 1000), new Material("Mora", MaterailTypes.OTHER, 79050 / 5) } },
                { "54", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 81650 / 1000), new Material("Mora", MaterailTypes.OTHER, 81650 / 5) } },
                { "55", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 84275 / 1000), new Material("Mora", MaterailTypes.OTHER, 84275 / 5) } },
                { "56", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 86950 / 1000), new Material("Mora", MaterailTypes.OTHER, 86950 / 5) } },
                { "57", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 89650 / 1000), new Material("Mora", MaterailTypes.OTHER, 89650 / 5) } },
                { "58", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 92400 / 1000), new Material("Mora", MaterailTypes.OTHER, 92400 / 5) } },
                { "59", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 95175 / 1000), new Material("Mora", MaterailTypes.OTHER, 95175 / 5) } },
                { "60", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 98000 / 1000), new Material("Mora", MaterailTypes.OTHER, 98000 / 5) } },
                { "61", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 100875 / 1000), new Material("Mora", MaterailTypes.OTHER, 100875 / 5) } },
                { "62", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 108950 / 1000), new Material("Mora", MaterailTypes.OTHER, 108950 / 5) } },
                { "63", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 112050 / 1000), new Material("Mora", MaterailTypes.OTHER, 112050 / 5) } },
                { "64", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 115175 / 1000), new Material("Mora", MaterailTypes.OTHER, 115175 / 5) } },
                { "65", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 118325 / 1000), new Material("Mora", MaterailTypes.OTHER, 118325 / 5) } },
                { "66", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 121525 / 1000), new Material("Mora", MaterailTypes.OTHER, 121525 / 5) } },
                { "67", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 124775 / 1000), new Material("Mora", MaterailTypes.OTHER, 124775 / 5) } },
                { "68", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 128075 / 1000), new Material("Mora", MaterailTypes.OTHER, 128075 / 5) } },
                { "69", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 131400 / 1000), new Material("Mora", MaterailTypes.OTHER, 131400 / 5) } },
                { "70", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 134775 / 1000), new Material("Mora", MaterailTypes.OTHER, 134775 / 5) } },
                { "71", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 138175 / 1000), new Material("Mora", MaterailTypes.OTHER, 138175 / 5) } },
                { "72", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 148700 / 1000), new Material("Mora", MaterailTypes.OTHER, 148700 / 5) } },
                { "73", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 152375 / 1000), new Material("Mora", MaterailTypes.OTHER, 152375 / 5) } },
                { "74", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 156075 / 1000), new Material("Mora", MaterailTypes.OTHER, 156075 / 5) } },
                { "75", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 159825 / 1000), new Material("Mora", MaterailTypes.OTHER, 159825 / 5) } },
                { "76", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 163600 / 1000), new Material("Mora", MaterailTypes.OTHER, 163600 / 5) } },
                { "77", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 167425 / 1000), new Material("Mora", MaterailTypes.OTHER, 167425 / 5) } },
                { "78", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 171300 / 1000), new Material("Mora", MaterailTypes.OTHER, 171300 / 5) } },
                { "79", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 175225 / 1000), new Material("Mora", MaterailTypes.OTHER, 175225 / 5) } },
                { "80", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 179175 / 1000), new Material("Mora", MaterailTypes.OTHER, 179175 / 5) } },
                { "81", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 183175 / 1000), new Material("Mora", MaterailTypes.OTHER, 183175 / 5) } },
                { "82", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 216225 / 1000), new Material("Mora", MaterailTypes.OTHER, 216225 / 5) } },
                { "83", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 243025 / 1000), new Material("Mora", MaterailTypes.OTHER, 243025 / 5) } },
                { "84", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 273100 / 1000), new Material("Mora", MaterailTypes.OTHER, 273100 / 5) } },
                { "85", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 306800 / 1000), new Material("Mora", MaterailTypes.OTHER, 306800 / 5) } },
                { "86", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 344600 / 1000), new Material("Mora", MaterailTypes.OTHER, 344600 / 5) } },
                { "87", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 386950 / 1000), new Material("Mora", MaterailTypes.OTHER, 386950 / 5) } },
                { "88", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 434425 / 1000), new Material("Mora", MaterailTypes.OTHER, 434425 / 5) } },
                { "89", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 487625 / 1000), new Material("Mora", MaterailTypes.OTHER, 487625 / 5) } },
                { "90", new Material[] { new Material("WanderersAdvice", MaterailTypes.EXP, 547200 / 1000), new Material("Mora", MaterailTypes.OTHER, 547200 / 5) } },
                //перенести в файл

                { "20+", new Material[] { new Material($"{Gem.GetMaterial(character, "green")}",  MaterailTypes.GEM, 1),  new Material($"{character.Assets.LocalSpecialty}", MaterailTypes.OTHER, 3)                                                                         ,  new Material($"{Enemy.GetMaterial(character, "white")}", MaterailTypes.ENEMY, 3),    new Material("Mora", MaterailTypes.OTHER, 20000) } },
                { "40+", new Material[] { new Material($"{Gem.GetMaterial(character, "blue")}",   MaterailTypes.GEM, 3),  new Material($"{character.Assets.LocalSpecialty}", MaterailTypes.OTHER, 10),  new Material($"{character.Assets.MiniBoss}", MaterailTypes.OTHER, 2) ,  new Material($"{Enemy.GetMaterial(character, "white")}", MaterailTypes.ENEMY, 15),   new Material("Mora", MaterailTypes.OTHER, 40000) } },
                { "50+", new Material[] { new Material($"{Gem.GetMaterial(character, "blue")}",   MaterailTypes.GEM, 6),  new Material($"{character.Assets.LocalSpecialty}", MaterailTypes.OTHER, 20),  new Material($"{character.Assets.MiniBoss}", MaterailTypes.OTHER, 4) ,  new Material($"{Enemy.GetMaterial(character, "green")}", MaterailTypes.ENEMY, 12),   new Material("Mora", MaterailTypes.OTHER, 60000) } },
                { "60+", new Material[] { new Material($"{Gem.GetMaterial(character, "violet")}", MaterailTypes.GEM, 3),  new Material($"{character.Assets.LocalSpecialty}", MaterailTypes.OTHER, 30),  new Material($"{character.Assets.MiniBoss}", MaterailTypes.OTHER, 8) ,  new Material($"{Enemy.GetMaterial(character, "green")}", MaterailTypes.ENEMY, 18),   new Material("Mora", MaterailTypes.OTHER, 80000) } },
                { "70+", new Material[] { new Material($"{Gem.GetMaterial(character, "violet")}", MaterailTypes.GEM, 6),  new Material($"{character.Assets.LocalSpecialty}", MaterailTypes.OTHER, 45),  new Material($"{character.Assets.MiniBoss}", MaterailTypes.OTHER, 12),  new Material($"{Enemy.GetMaterial(character, "blue")}",  MaterailTypes.ENEMY, 12),   new Material("Mora", MaterailTypes.OTHER, 100000) } },
                { "80+", new Material[] { new Material($"{Gem.GetMaterial(character, "orange")}", MaterailTypes.GEM, 6),  new Material($"{character.Assets.LocalSpecialty}", MaterailTypes.OTHER, 60),  new Material($"{character.Assets.MiniBoss}", MaterailTypes.OTHER, 20),  new Material($"{Enemy.GetMaterial(character, "blue")}",  MaterailTypes.ENEMY, 24),   new Material("Mora", MaterailTypes.OTHER, 120000) } }
            };
        }


        private static readonly List<string> Levels = new()  {
            "1"  ,  "2" ,  "3",  "4" , "5" , "6" , "7" , "8" , "9" , "10",
            "11" ,  "12", "13",  "14", "15", "16", "17", "18", "19", "20", "20+",
            "21" ,  "22", "23",  "24", "25", "26", "27", "28", "29", "30",
            "31" ,  "32", "33",  "34", "35", "36", "37", "38", "39", "40", "40+",
            "41" ,  "42", "43",  "44", "45", "46", "47", "48", "49", "50", "50+",
            "51" ,  "52", "53",  "54", "55", "56", "57", "58", "59", "60", "60+",
            "61" ,  "62", "63",  "64", "65", "66", "67", "68", "69", "70", "70+",
            "71" ,  "72", "73",  "74", "75", "76", "77", "78", "79", "80", "80+",
            "81" ,  "82", "83",  "84", "85", "86", "87", "88", "89", "90"
        };
    }
}