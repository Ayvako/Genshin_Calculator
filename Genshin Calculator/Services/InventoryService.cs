using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.Interfaces;
using Genshin_Calculator.Services.Materials;

namespace Genshin_Calculator.Services;

public class InventoryService
{
    private readonly IInventoryStore store;

    private readonly SkillMaterialProvider books;

    private readonly GemMaterialProvider gems;

    private readonly EnemyMaterialProvider enemies;

    private readonly ISkillUpgradeService skillUpgrade;

    private readonly ICharacterUpgradeService characterUpgrade;

    public InventoryService(
        SkillMaterialProvider books,
        GemMaterialProvider gems,
        EnemyMaterialProvider enemies,
        IInventoryStore store,
        ISkillUpgradeService skillUpgrade,
        ICharacterUpgradeService characterUpgrade)
    {
        this.skillUpgrade = skillUpgrade;
        this.store = store;
        this.books = books;
        this.gems = gems;
        this.enemies = enemies;
        this.characterUpgrade = characterUpgrade;
    }

    public void Upgrade(Character character, Inventory inventory)
    {
        var missingMap = this.CalculateMissingMaterials(inventory);
        var missing = missingMap.TryGetValue(character, out List<Material>? value) ? value : [];

        if (missing.Count == 0)
        {
            character.CurrentLevel = character.DesiredLevel;
            character.AutoAttack.CurrentLevel = character.AutoAttack.DesiredLevel;
            character.Elemental.CurrentLevel = character.Elemental.DesiredLevel;
            character.Burst.CurrentLevel = character.Burst.DesiredLevel;
        }
        else
        {
            Debug.WriteLine($"{character.Name}: Cannot upgrade, missing materials.");
        }
    }

    public Inventory GetInventory() => this.store.Inventory;

    public IReadOnlyList<Character> GetCharacters()
    {
        return this.GetInventory().Characters;
    }

    public Dictionary<Character, List<Material>> CalculateMissingMaterials(Inventory sourceInventory)
    {
        var tempInventory = sourceInventory.Clone();

        var result = new Dictionary<Character, List<Material>>();

        long totalExpPool = CalculateTotalExp(tempInventory);

        var sortedCharacters = sourceInventory.NotDeletedCharacters
        .OrderBy(c => c.Priority)
        .ToList();

        foreach (var character in sortedCharacters)
        {
            if (!character.Activated)
            {
                continue;
            }

            var missingForChar = new List<Material>();

            var requiredMaterials = this.TotalCost(character).OrderByDescending(m => m.Rarity);

            foreach (var required in requiredMaterials)
            {
                switch (required.Type)
                {
                    case MaterialTypes.Exp:

                        if (totalExpPool >= required.Amount)
                        {
                            totalExpPool -= (long)required.Amount;
                        }
                        else
                        {
                            long shortageXP = (long)required.Amount - totalExpPool;

                            int booksNeeded = (int)Math.Ceiling(shortageXP / 20000.0);

                            if (booksNeeded > 0)
                            {
                                missingForChar.Add(new Material(
                                    "HerosWit",
                                    MaterialTypes.Exp,
                                    MaterialRarity.Violet,
                                    booksNeeded));
                            }

                            totalExpPool = 0;
                        }

                        break;

                    case MaterialTypes.Book:
                    case MaterialTypes.Gem:
                    case MaterialTypes.Enemy:
                        this.ConsumeWithCascade(tempInventory, required, character, missingForChar);
                        break;

                    default:
                        ConsumeDirectly(tempInventory, required, missingForChar);
                        break;
                }
            }

            if (missingForChar.Count > 0)
            {
                result[character] = SortMaterialsForDisplay(missingForChar);
            }
        }

        return result;
    }

    private static List<Material> SortMaterialsForDisplay(List<Material> materials)
    {
        return [.. materials
            .OrderBy(m => GetTypePriority(m.Type))
            .ThenBy(m => m.Rarity)];
    }

    private static int GetTypePriority(MaterialTypes type) => type switch
    {
        MaterialTypes.Gem => 1,
        MaterialTypes.WeeklyBoss => 2,
        MaterialTypes.MiniBoss => 3,
        MaterialTypes.LocalSpecialty => 4,
        MaterialTypes.Enemy => 5,
        MaterialTypes.Book => 6,
        MaterialTypes.Mora => 99,
        MaterialTypes.Exp => 100,
        _ => 50,
    };

    private static void ConsumeDirectly(Inventory inventory, Material required, List<Material> missingList)
    {
        var item = inventory.GetMaterial(required.Name);
        int available = item?.Amount ?? 0;

        if (available >= required.Amount)
        {
            inventory.SetMaterial(new Material(required.Name, required.Type, required.Rarity, available - required.Amount));
        }
        else
        {
            inventory.SetMaterial(new Material(required.Name, required.Type, required.Rarity, 0));
            missingList.Add(new Material(required.Name, required.Type, required.Rarity, required.Amount - available));
        }
    }

    private static long CalculateTotalExp(Inventory inv)
    {
        return ((long)(inv.GetMaterial("HerosWit")?.Amount ?? 0) * 20000)
             + ((long)(inv.GetMaterial("AdventurersExperience")?.Amount ?? 0) * 5000)
             + ((long)(inv.GetMaterial("WanderersAdvice")?.Amount ?? 0) * 1000);
    }

    private static List<MaterialRarity> GetRarityChain(MaterialTypes type) => type switch
    {
        MaterialTypes.Gem => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet, MaterialRarity.Orange],
        MaterialTypes.Book => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet],
        MaterialTypes.Enemy => [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue],
        _ => [],
    };

    private void ConsumeWithCascade(Inventory inventory, Material required, Character character, List<Material> missingList)
    {
        var chain = GetRarityChain(required.Type);

        int tierIndex = chain.IndexOf(required.Rarity);
        if (tierIndex == -1)
        {
            return;
        }

        int remainingNeeded = this.ProcessTier(inventory, character, required.Type, chain, tierIndex, required.Amount);

        if (remainingNeeded > 0)
        {
            string name = this.GetMaterialName(character, required.Type, required.Rarity);
            missingList.Add(new Material(name, required.Type, required.Rarity, remainingNeeded));
        }
    }

    private int ProcessTier(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int tierIndex, int amountNeeded)
    {
        if (amountNeeded <= 0)
        {
            return 0;
        }

        MaterialRarity currentRarity = chain[tierIndex];
        string name = this.GetMaterialName(character, type, currentRarity);

        var itemInStock = inventory.GetMaterial(name);
        int available = itemInStock?.Amount ?? 0;

        int take = Math.Min(available, amountNeeded);
        if (take > 0)
        {
            inventory.SetMaterial(new Material(name, type, currentRarity, available - take));
            amountNeeded -= take;
        }

        if (amountNeeded == 0)
        {
            return 0;
        }

        if (tierIndex > 0)
        {
            int neededFromLowerTier = amountNeeded * 3;
            int remainingFromLower = this.ProcessTier(inventory, character, type, chain, tierIndex - 1, neededFromLowerTier);
            int crafted = (neededFromLowerTier - remainingFromLower) / 3;
            amountNeeded -= crafted;
        }

        return amountNeeded;
    }

    private string GetMaterialName(Character c, MaterialTypes type, MaterialRarity rarity) =>
       type switch
       {
           MaterialTypes.Gem => this.gems.GetMaterial(c, rarity),
           MaterialTypes.Book => this.books.GetMaterial(c, rarity),
           MaterialTypes.Enemy => this.enemies.GetMaterial(c, rarity),
           _ => throw new ArgumentException($"Unknown material type {type}"),
       };

    private List<Material> TotalCost(Character character)
    {
        var charCost = this.characterUpgrade.GetCharacterCost(character);
        var skillCost = this.skillUpgrade.GetSkillsCost(character);

        var result = new Dictionary<string, Material>();

        foreach (var list in new[] { charCost, skillCost })
        {
            foreach (var m in list)
            {
                if (result.TryGetValue(m.Name, out var existing))
                {
                    existing.Amount += m.Amount;
                }
                else
                {
                    result[m.Name] = m.Clone();
                }
            }
        }

        return [.. result.Values];
    }
}