using System;
using System.Collections.Generic;
using System.Linq;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using Genshin_Calculator.Services.Interfaces;
using Genshin_Calculator.Services.MaterialProviders;

namespace Genshin_Calculator.Services;

public class CharacterUpgradeService : ICharacterUpgradeService
{
    private static readonly List<string> Levels = [.. LevelHelper.Levels];

    private readonly GemMaterialProvider gems;

    private readonly EnemyMaterialProvider enemies;

    public CharacterUpgradeService(GemMaterialProvider gems, EnemyMaterialProvider enemies)
    {
        this.gems = gems;
        this.enemies = enemies;
    }

    public List<Material> GetCharacterCost(Character character)
    {
        return this.GetCost(character, character.CurrentLevel, character.DesiredLevel);
    }

    private List<Material> GetCost(Character character, string from, string to)
    {
        List<Material> requiredMaterials = [];
        Dictionary<string, Material[]> amountMaterials = this.GetMaterials(character);

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

    private Dictionary<string, Material[]> GetMaterials(Character character)
    {
        ArgumentNullException.ThrowIfNull(character);

        ArgumentNullException.ThrowIfNull(character.Assets);

        const string WanderersAdvice = "WanderersAdvice";

        const string Mora = "Mora";

        return new()
        {
            { "2", new Material[] { new (WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 1), new (Mora, MaterialTypes.Other, MaterialRarity.Blue, 200) } },
            { "3", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 1325), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 1325 / 5) } },
            { "4", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 1700), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 1700 / 5) } },
            { "5", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 2150), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 2150 / 5) } },
            { "6", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 2625), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 2625 / 5) } },
            { "7", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 3150), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 3150 / 5) } },
            { "8", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 3725), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 3725 / 5) } },
            { "9", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 4350), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 4350 / 5) } },
            { "10", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 5000), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 5000 / 5) } },
            { "11", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 5700), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 5700 / 5) } },
            { "12", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 6450), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 6450 / 5) } },
            { "13", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 7225), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 7225 / 5) } },
            { "14", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 8050), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 8050 / 5) } },
            { "15", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 8925), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 8925 / 5) } },
            { "16", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 9825), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 9825 / 5) } },
            { "17", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 10750), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 10750 / 5) } },
            { "18", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 11725), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 11725 / 5) } },
            { "19", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 12725), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 12725 / 5) } },
            { "20", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 13775), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 13775 / 5) } },
            { "21", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 14875), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 14875 / 5) } },
            { "22", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 16800), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 16800 / 5) } },
            { "23", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 18000), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 18000 / 5) } },
            { "24", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 19250), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 19250 / 5) } },
            { "25", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 20550), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 20550 / 5) } },
            { "26", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 21875), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 21875 / 5) } },
            { "27", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 23250), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 23250 / 5) } },
            { "28", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 24650), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 24650 / 5) } },
            { "29", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 26100), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 26100 / 5) } },
            { "30", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 27575), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 27575 / 5) } },
            { "31", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 29100), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 29100 / 5) } },
            { "32", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 30650), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 30650 / 5) } },
            { "33", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 32250), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 32250 / 5) } },
            { "34", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 33875), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 33875 / 5) } },
            { "35", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 35550), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 35550 / 5) } },
            { "36", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 37250), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 37250 / 5) } },
            { "37", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 38975), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 38975 / 5) } },
            { "38", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 40750), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 40750 / 5) } },
            { "39", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 42575), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 42575 / 5) } },
            { "40", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 44425), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 44425 / 5) } },
            { "41", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 46300), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 50625 / 5) } },
            { "42", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 50625), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 50625 / 5) } },
            { "43", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 52700), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 52700 / 5) } },
            { "44", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 54775), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 54775 / 5) } },
            { "45", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 56900), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 56900 / 5) } },
            { "46", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 59075), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 59075 / 5) } },
            { "47", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 61275), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 61275 / 5) } },
            { "48", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 63525), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 63525 / 5) } },
            { "49", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 65800), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 65800 / 5) } },
            { "50", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 68125), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 68125 / 5) } },
            { "51", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 70475), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 70475 / 5) } },
            { "52", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 76500), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 76500 / 5) } },
            { "53", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 79050), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 79050 / 5) } },
            { "54", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 81650), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 81650 / 5) } },
            { "55", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 84275), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 84275 / 5) } },
            { "56", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 86950), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 86950 / 5) } },
            { "57", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 89650), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 89650 / 5) } },
            { "58", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 92400), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 92400 / 5) } },
            { "59", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 95175), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 95175 / 5) } },
            { "60", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 98000), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 98000 / 5) } },
            { "61", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 100875), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 100875 / 5) } },
            { "62", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 108950), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 108950 / 5) } },
            { "63", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 112050), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 112050 / 5) } },
            { "64", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 115175), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 115175 / 5) } },
            { "65", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 118325), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 118325 / 5) } },
            { "66", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 121525), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 121525 / 5) } },
            { "67", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 124775), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 124775 / 5) } },
            { "68", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 128075), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 128075 / 5) } },
            { "69", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 131400), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 131400 / 5) } },
            { "70", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 134775), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 134775 / 5) } },
            { "71", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 138175), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 138175 / 5) } },
            { "72", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 148700), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 148700 / 5) } },
            { "73", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 152375), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 152375 / 5) } },
            { "74", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 156075), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 156075 / 5) } },
            { "75", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 159825), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 159825 / 5) } },
            { "76", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 163600), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 163600 / 5) } },
            { "77", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 167425), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 167425 / 5) } },
            { "78", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 171300), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 171300 / 5) } },
            { "79", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 175225), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 175225 / 5) } },
            { "80", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 179175), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 179175 / 5) } },
            { "81", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 183175), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 183175 / 5) } },
            { "82", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 216225), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 216225 / 5) } },
            { "83", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 243025), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 243025 / 5) } },
            { "84", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 273100), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 273100 / 5) } },
            { "85", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 306800), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 306800 / 5) } },
            { "86", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 344600), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 344600 / 5) } },
            { "87", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 386950), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 386950 / 5) } },
            { "88", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 434425), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 434425 / 5) } },
            { "89", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 487625), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 487625 / 5) } },
            { "90", new Material[] { new(WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 547200), new(Mora, MaterialTypes.Other, MaterialRarity.Blue, 547200 / 5) } },
            { "20★", new Material[] { new($"{this.gems.GetMaterial(character, MaterialRarity.Green)}", MaterialTypes.Gem, MaterialRarity.Green, 1), new($"{character.Assets!.LocalSpecialty}", MaterialTypes.Other, MaterialRarity.White, 3), new($"{this.enemies.GetMaterial(character, MaterialRarity.White)}", MaterialTypes.Enemy, MaterialRarity.White, 3), new("Mora", MaterialTypes.Other, MaterialRarity.Blue, 20000) } },
            { "40★", new Material[] { new($"{this.gems.GetMaterial(character, MaterialRarity.Blue)}", MaterialTypes.Gem, MaterialRarity.Blue, 3), new($"{character.Assets!.LocalSpecialty}", MaterialTypes.Other, MaterialRarity.White, 10), new($"{character.Assets.MiniBoss}", MaterialTypes.Other, MaterialRarity.Violet, 2), new($"{this.enemies.GetMaterial(character, MaterialRarity.White)}", MaterialTypes.Enemy, MaterialRarity.White, 15), new("Mora", MaterialTypes.Other, MaterialRarity.Blue, 40000) } },
            { "50★", new Material[] { new($"{this.gems.GetMaterial(character, MaterialRarity.Blue)}", MaterialTypes.Gem, MaterialRarity.Blue, 6), new($"{character.Assets!.LocalSpecialty}", MaterialTypes.Other, MaterialRarity.White, 20), new($"{character.Assets.MiniBoss}", MaterialTypes.Other, MaterialRarity.Violet, 4), new($"{this.enemies.GetMaterial(character, MaterialRarity.Green)}", MaterialTypes.Enemy, MaterialRarity.Green, 12), new("Mora", MaterialTypes.Other, MaterialRarity.Blue, 60000) } },
            { "60★", new Material[] { new($"{this.gems.GetMaterial(character, MaterialRarity.Violet)}", MaterialTypes.Gem, MaterialRarity.Violet, 3), new($"{character.Assets!.LocalSpecialty}", MaterialTypes.Other, MaterialRarity.White, 30), new($"{character.Assets.MiniBoss}", MaterialTypes.Other, MaterialRarity.Violet, 8), new($"{this.enemies.GetMaterial(character, MaterialRarity.Green)}", MaterialTypes.Enemy, MaterialRarity.Green, 18), new("Mora", MaterialTypes.Other, MaterialRarity.Blue, 80000) } },
            { "70★", new Material[] { new($"{this.gems.GetMaterial(character, MaterialRarity.Violet)}", MaterialTypes.Gem, MaterialRarity.Violet, 6), new($"{character.Assets!.LocalSpecialty}", MaterialTypes.Other, MaterialRarity.White, 45), new($"{character.Assets.MiniBoss}", MaterialTypes.Other, MaterialRarity.Violet, 12), new($"{this.enemies.GetMaterial(character, MaterialRarity.Blue)}", MaterialTypes.Enemy, MaterialRarity.Blue, 12), new("Mora", MaterialTypes.Other, MaterialRarity.Blue, 100000) } },
            { "80★", new Material[] { new($"{this.gems.GetMaterial(character, MaterialRarity.Orange)}", MaterialTypes.Gem, MaterialRarity.Orange, 6), new($"{character.Assets!.LocalSpecialty}", MaterialTypes.Other, MaterialRarity.White, 60), new($"{character.Assets.MiniBoss}", MaterialTypes.Other, MaterialRarity.Violet, 20), new($"{this.enemies.GetMaterial(character, MaterialRarity.Blue)}", MaterialTypes.Enemy, MaterialRarity.Blue, 24), new("Mora", MaterialTypes.Other, MaterialRarity.Blue, 120000) } },
        };
    }
}