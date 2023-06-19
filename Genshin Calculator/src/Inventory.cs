using Genshin.src.LevelingResources;
using Genshin.src.Upgrades;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genshin.src
{
    public static class Inventory
    {
        public static Dictionary<string, int> MyInventory = new();
        public static Dictionary<string, int> InventoryCopy = new();

        public static List<Character> Characters = new();

        static Dictionary<Character, List<Material>> RequiredMaterials = new();


        public static Dictionary<Character, List<Material>> CalcRequiredMaterials()
        {
            List<Character> activeCharacters = GetActiveCharacters();
            Dictionary<Character, List<Material>> materialsForCharacters = new();
            List<Material> remainingMaterials = new();


            int exp = (InventoryCopy["HerosWit"] * 20 + InventoryCopy["AdventurersExperience"] * 5 + InventoryCopy["WanderersAdvice"] * 1);
            foreach (var character in activeCharacters)
            {

                foreach (var m in TotalCost(character).OrderBy(m => m.Type).ToList())
                {

                    switch (m.Type)
                    {

                        case MaterailTypes.EXP:
                            exp = CalcExp(m, remainingMaterials, exp);
                            break;
                        case MaterailTypes.BOOK:
                        case MaterailTypes.GEM:
                        case MaterailTypes.ENEMY:
                            CalcAlchemistMaterials(m, character, remainingMaterials);
                            break;
                        default:
                            CalcMaterials(m, remainingMaterials);
                            break;
                    }
                    
                }

                materialsForCharacters.Add(character, new List<Material>(remainingMaterials));

                remainingMaterials.Clear();
            }
            return materialsForCharacters;
        }
        public static void CalcAlchemistMaterials(Material requiredMaterial, Character c,  List<Material> remainingMaterials)
        {
            string greenMaterial = "";
            string blueMaterial = "";
            string violetMaterial = "";
            string orangeMaterial ="";

            switch (requiredMaterial.Type)
            {

                case MaterailTypes.GEM:
                    greenMaterial = Gem.GetMaterial(c, "green");
                    blueMaterial = Gem.GetMaterial(c, "blue");
                    violetMaterial = Gem.GetMaterial(c, "violet");
                    orangeMaterial = Gem.GetMaterial(c, "orange");
                    break;
                case MaterailTypes.BOOK:
                    greenMaterial = Book.GetMaterial(c, "green");
                    blueMaterial = Book.GetMaterial(c, "blue");
                    violetMaterial = Book.GetMaterial(c, "violet");
                    break;
                default:
                    greenMaterial = Enemy.GetMaterial(c, "white");
                    blueMaterial = Enemy.GetMaterial(c, "green");
                    violetMaterial = Enemy.GetMaterial(c, "blue");
                    break;
            }




                if (requiredMaterial.Name == greenMaterial)
                {
                    int green = InventoryCopy[requiredMaterial.Name];

                    if (green < requiredMaterial.Amount)//Ресурса не хваватает
                    {
                        remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Amount - green));
                        InventoryCopy[requiredMaterial.Name] = 0;
                    }
                    else//Ресурса хваватает
                    {
                        remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                        InventoryCopy[requiredMaterial.Name] -= requiredMaterial.Amount;
                    }
                }
                else if (requiredMaterial.Name == blueMaterial)
                {
                    int blue = InventoryCopy[requiredMaterial.Name];
                    if (blue < requiredMaterial.Amount)//Ресурса не хваватает
                    {
                        int green = InventoryCopy[greenMaterial];
                        int blue_alchemist = green / 3;

                        if (blue_alchemist + blue < requiredMaterial.Amount)//Ресурса не хваватает с алхимией
                        {
                            remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Amount - (blue_alchemist + blue)));
                            InventoryCopy[greenMaterial] -= (blue_alchemist) * 3;
                            InventoryCopy[requiredMaterial.Name] = 0;
                        }
                        else//Ресурса хваватает с алхимией
                        {
                            remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                            InventoryCopy[greenMaterial] -= (requiredMaterial.Amount - blue) * 3;
                            InventoryCopy[requiredMaterial.Name] -= blue;
                        }
                    }
                    else//Ресурса хваватает
                    {

                        remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                        InventoryCopy[requiredMaterial.Name] -= requiredMaterial.Amount;
                    }
                }
                else if (requiredMaterial.Name == violetMaterial)
                {
                    int violet = InventoryCopy[requiredMaterial.Name];

                    if (violet < requiredMaterial.Amount)//Ресурса не хваватает
                    {
                        int green = InventoryCopy[greenMaterial];
                        int blue = InventoryCopy[blueMaterial];
                        int blueAlchemist = green / 3;
                        int violetAlchemist = (blueAlchemist + blue) / 3;
                        if (violetAlchemist + violet < requiredMaterial.Amount)//Ресурса не хваватает с алхимией
                        {
                            remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Amount - (violetAlchemist + violet)));
                            InventoryCopy[greenMaterial] -= (blueAlchemist - blueAlchemist % 3) * 3;
                            InventoryCopy[blueMaterial] -= (blue - blue % 3);
                            InventoryCopy[requiredMaterial.Name] = 0;
                        }
                        else //Ресурса хваватает с алхимией
                        {
                            remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                            int blueToViolet = (requiredMaterial.Amount - violet) * 3 - blue;
                            int violetToViolet = (requiredMaterial.Amount - violet);

                            InventoryCopy[greenMaterial] -= blueToViolet <= 0 ? 0 : blueToViolet * 3;
                            InventoryCopy[blueMaterial] -= blueToViolet <= 0 ? violetToViolet * 3 : blue;
                            InventoryCopy[requiredMaterial.Name] -= violet;
                        }
                    }
                    else//Ресурса хваватает
                    {
                        remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                        InventoryCopy[requiredMaterial.Name] -= requiredMaterial.Amount;
                    }

                }
                else if (requiredMaterial.Name == orangeMaterial)
                {
                    int orange = InventoryCopy[requiredMaterial.Name];

                    if (orange < requiredMaterial.Amount)//Ресурса не хваватает
                    {

                        int green = InventoryCopy[greenMaterial];
                        int blue = InventoryCopy[blueMaterial];
                        int violet = InventoryCopy[violetMaterial];
                        int blueAlchemist = green / 3;
                        int violetAlchemist = (blue + blueAlchemist) / 3;
                        int orangeAlchemist = (violet + violetAlchemist) / 3;

                        if (orangeAlchemist + orange < requiredMaterial.Amount)//Ресурса не хваватает с алхимией
                        {
                            
                            remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Amount - (orangeAlchemist + orange)));
                            InventoryCopy[greenMaterial] -= green - green % 27;
                            InventoryCopy[blueMaterial] -= blue - blue % 9;
                            InventoryCopy[violetMaterial] -= violet - violet % 3;

                            green = InventoryCopy[greenMaterial];
                            blue = InventoryCopy[blueMaterial];
                            violet = InventoryCopy[violetMaterial];

                            bool isOrange = ((green / 3 + blue) / 3 + violet) / 3 >= 1;

                            InventoryCopy[greenMaterial] -= isOrange ? ((3 - violet) * 3 - blue) * 3 : 0;
                            InventoryCopy[blueMaterial] -= isOrange ? blue : 0;
                            InventoryCopy[violetMaterial] -= isOrange ?  violet : 0;

                            InventoryCopy[requiredMaterial.Name] = 0;


                        }
                        else //Ресурса хваватает с алхимией
                        {
                            remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                           
                            int blueToOrange = ((requiredMaterial.Amount - orange) * 3 - violet) * 3 - blue;
                            int violetToOrange = (requiredMaterial.Amount - orange) * 3 - violet;
                            int orangeToToOrange = requiredMaterial.Amount - orange;

                            InventoryCopy[greenMaterial] -= blueToOrange <= 0 ? 0 : blueToOrange * 3;
                            InventoryCopy[blueMaterial] -= blueToOrange <= 0 ? violetToOrange <= 0 ? 0 : violetToOrange * 3 : blue;
                            InventoryCopy[violetMaterial] -= violetToOrange <= 0 ? orangeToToOrange * 3 : violet;

                            InventoryCopy[requiredMaterial.Name] -= orange;

                        }
                    }
                    else//Ресурса хваватает
                    {
                        remainingMaterials.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                        InventoryCopy[requiredMaterial.Name] -= requiredMaterial.Amount;

                    }

                }

        }
        public static int CalcExp(Material requiredMaterial, List<Material> inventory, int exp)
        {
            if (requiredMaterial.Name == "WanderersAdvice" && exp < requiredMaterial.Amount)
            {
                inventory.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Amount - exp));
                exp = 0;
            }
            else
            {
                inventory.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, 0));
                exp -= requiredMaterial.Amount;
            }
            return exp;
        }

        public static void CalcMaterials(Material m, List<Material> remainingMaterials)
        {
            if (InventoryCopy[m.Name] < m.Amount)
            {
                remainingMaterials.Add(new Material(m.Name, m.Type, m.Amount - InventoryCopy[m.Name]));
                InventoryCopy[m.Name] = 0;
            }
            else
            {
                remainingMaterials.Add(new Material(m.Name, m.Type, 0));
                InventoryCopy[m.Name] -= m.Amount;
            }
        }


        public static void AddCharacter(Character character) => character.Deleted = false;
        public static void CharacrerLevelUp(Character character, string to) => character.DesiredLevel = to;
        public static void AAChangeLevel(Character character, int to) => character.AutoAttack.DesiredLevel = to;
        public static void ElemChangeLevel(Character character, int to) => character.Elemental.DesiredLevel = to;
        public static void BurstChangeLevel(Character character, int to) => character.Burst.DesiredLevel = to;
        public static List<Character> GetAllCharacters()
        {
            return Characters;
        }
        public static List<Character> GetActiveCharacters()
        {
            if (Characters != null)
                return Characters.FindAll(c => c.Activated == true);
            else return new List<Character>();
        }
        public static List<Character> GetDisabledCharacters()
        {
            if (Characters != null)
                return Characters.FindAll(c => c.Activated == false);
            else return new List<Character>();
        }
        public static List<Character> GetDeletedCharacters()
        {
            if (Characters != null)
                return Characters.FindAll(c => c.Deleted == true);
            else return new List<Character>();
        }
        public static List<Character> GetNotDeletedCharacters()
        {
            if (Characters != null)
                return Characters.FindAll(c => c.Deleted == false);
            else return new List<Character>();
        }
        public static void Upgrade(Character character)
        {
            RequiredMaterials = CalcRequiredMaterials();
            if (IsUpgradable(RequiredMaterials[character]))
            {
                character.CurrentLevel = character.DesiredLevel;

                Skill auto_attack = character.AutoAttack;
                Skill elemental = character.Elemental;
                Skill burst = character.Burst;

                auto_attack.CurrentLevel = auto_attack.DesiredLevel;
                elemental.CurrentLevel = elemental.DesiredLevel;
                burst.CurrentLevel = burst.DesiredLevel;

                foreach (var m in RequiredMaterials[character])
                    MyInventory[m.Name] -= m.Amount;

                RequiredMaterials = CalcRequiredMaterials();
                InventoryCopy = CopyDictionary(MyInventory);
            }
            else Debug.WriteLine($"{character} Error Upgrade");
        }


        public static void ChangePriority(Character c1, Character c2) => (c2.Priority, c1.Priority) = (c1.Priority, c2.Priority);
        public static void AddMaterial(Material material, int amount) => material.Amount += amount;
        public static void SetMaterial(Material material, int amount) => material.Amount = amount;
        public static void EnableCharacter(Character character) => character.Activated = true;
        public static void DisableCharacter(Character character) => character.Activated = false;
        public static void DeleteCharacter(Character character) => character.Deleted = true;
        private static bool IsUpgradable(List<Material> materials)
        {
            return materials.All(m => m.Amount == 0);
        }
        public static Dictionary<string, int> CopyDictionary(Dictionary<string, int> old)
        {
            return new Dictionary<string, int>(old);
        }

        private static List<Material> CharacterCost(Character character)
        {
            return CharacterUpgrade.GetCost(character, character.CurrentLevel, character.DesiredLevel);
        }

        private static List<Material> TotalCost(Character character)
        {
            return MergeDictionaries(CharacterCost(character), SkillsCost(character));
        }

        private static List<Material> SkillsCost(Character character)
        {
            Skill auto_attack = character.AutoAttack;
            Skill elemental = character.Elemental;
            Skill burst = character.Burst;

            List<Material> materialsForAA = SkillUpgrade.GetCost(character, auto_attack.CurrentLevel, auto_attack.DesiredLevel);
            List<Material> materialsForElem = SkillUpgrade.GetCost(character, elemental.CurrentLevel, elemental.DesiredLevel);
            List<Material> materialsForBurst = SkillUpgrade.GetCost(character, burst.CurrentLevel, burst.DesiredLevel);

            return MergeDictionaries(materialsForAA, materialsForElem, materialsForBurst);
        }
        private static List<Material> MergeDictionaries(params List<Material>[] dictionaries)
        {
            IEnumerable<Material> merged = dictionaries[0];
            for (int i = 1; i < dictionaries.Length; i++)
                merged = merged.Concat(dictionaries[i]);

            var groupedMaterials = merged.GroupBy(m => new { m.Name})
                .Select(g => new Material(g.Key.Name, g.First().Type, g.Sum(m => m.Amount)))
                .ToList();

            return groupedMaterials;
        }
    }
}