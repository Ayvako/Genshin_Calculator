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

    private readonly BookMaterialProvider books;

    private readonly GemMaterialProvider gems;

    private readonly EnemyMaterialProvider enemies;

    private readonly ISkillUpgradeService skillUpgrade;

    private readonly ICharacterUpgradeService characterUpgrade;

    public InventoryService(
        BookMaterialProvider books,
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

    public Inventory GetInventory()
    {
        return this.store.Inventory;
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return this.GetInventory().Characters;
    }

    public Dictionary<Character, List<Material>> CalculateMissingMaterials(Inventory sourceInventory)
    {
        var missingMaterialsByCharacter = new Dictionary<Character, List<Material>>();
        var inventoryCopy = sourceInventory.Clone();
        var missingMaterials = new List<Material>();

        int totalExp = ((inventoryCopy.GetMaterial("HerosWit")?.Amount ?? 0) * 20)
                + ((inventoryCopy.GetMaterial("AdventurersExperience")?.Amount ?? 0) * 5)
                + ((inventoryCopy.GetMaterial("WanderersAdvice")?.Amount ?? 0) * 1);

        foreach (var character in sourceInventory.NotDeletedCharacters)
        {
            if (!character.Activated!)
            {
                continue;
            }

            foreach (var required in this.TotalCost(character).OrderBy(m => m.Type))
            {
                switch (required.Type)
                {
                    case MaterialTypes.Exp:
                        totalExp = ConsumeExperience(required, missingMaterials, totalExp);
                        break;

                    case MaterialTypes.Book:
                    case MaterialTypes.Gem:
                    case MaterialTypes.Enemy:
                        this.ConsumeCraftableMaterials(inventoryCopy, required, character, missingMaterials);
                        break;

                    default:
                        ConsumeMaterialFromInventory(inventoryCopy, required, missingMaterials);
                        break;
                }
            }

            missingMaterialsByCharacter[character] = [.. missingMaterials];
            missingMaterials.Clear();
        }

        return missingMaterialsByCharacter;
    }

    public void Upgrade(Character character, Inventory inventory)
    {
        var requiredMaterials = this.CalculateMissingMaterials(inventory);
        if (IsUpgradable(requiredMaterials[character]))
        {
            character.CurrentLevel = character.DesiredLevel;

            Skill autoAttack = character.AutoAttack;
            Skill elemental = character.Elemental;
            Skill burst = character.Burst;

            autoAttack.CurrentLevel = autoAttack.DesiredLevel;
            elemental.CurrentLevel = elemental.DesiredLevel;
            burst.CurrentLevel = burst.DesiredLevel;

            foreach (Material m in requiredMaterials[character])
            {
                inventory.SubtractMaterial(m);
            }
        }
        else
        {
            Debug.WriteLine($"{character} Error Upgrade");
        }
    }

    private static bool IsUpgradable(List<Material> materials)
    {
        return materials.All(m => m.Amount == 0);
    }

    private static void ConsumeMaterialFromInventory(Inventory inventory, Material m, List<Material> remainingMaterials)
    {
        var available = inventory.GetMaterial(m.Name)?.Amount ?? 0;

        if (available < m.Amount)
        {
            remainingMaterials.Add(new Material(m.Name, m.Type, m.Rarity, m.Amount - available));
            inventory.SetMaterial(new Material(m.Name, m.Type, m.Rarity, 0));
        }
        else
        {
            inventory.SetMaterial(new Material(m.Name, m.Type, m.Rarity, available - m.Amount));
            remainingMaterials.Add(new Material(m.Name, m.Type, m.Rarity, 0));
        }
    }

    private static int ConsumeExperience(Material requiredMaterial, List<Material> inventory, int exp)
    {
        if (requiredMaterial.Name == "WanderersAdvice" && exp < requiredMaterial.Amount)
        {
            inventory.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Rarity, requiredMaterial.Amount - exp));
            exp = 0;
        }
        else
        {
            inventory.Add(new Material(requiredMaterial.Name, requiredMaterial.Type, requiredMaterial.Rarity, 0));
            exp -= requiredMaterial.Amount;
        }

        return exp;
    }

    private static MaterialRarity? GetLowerColor(List<MaterialRarity> colors, MaterialRarity current)
    {
        int i = colors.IndexOf(current);
        return i > 0 ? colors[i - 1] : null;
    }

    private static void ConsumeWithCrafting(Inventory inventory, Dictionary<MaterialRarity, string> chain, MaterialRarity color, Material required, List<Material> shortages)
    {
        string materialName = chain[color];
        int available = inventory.GetMaterial(materialName)?.Amount ?? 0;

        if (available >= required.Amount)
        {
            inventory.SetMaterial(new(materialName, required.Type, required.Rarity, available - required.Amount));
            shortages.Add(new(materialName, required.Type, required.Rarity, 0));
            return;
        }

        int missing = required.Amount - available;
        MaterialRarity? lowerColor = GetLowerColor([.. chain.Keys], color);

        if (lowerColor is null)
        {
            shortages.Add(new(materialName, required.Type, required.Rarity, missing));
            inventory.SetMaterial(new(materialName, required.Type, required.Rarity, 0));
            return;
        }

        string lowerName = chain[lowerColor.Value];
        int lowerAvailable = inventory.GetMaterial(lowerName)?.Amount ?? 0;
        int canCraft = lowerAvailable / 3;
        int totalPossible = available + canCraft;

        if (totalPossible < required.Amount)
        {
            shortages.Add(new(materialName, required.Type, required.Rarity, required.Amount - totalPossible));

            inventory.SetMaterial(new(lowerName, required.Type, required.Rarity, lowerAvailable % 3));
            inventory.SetMaterial(new(materialName, required.Type, required.Rarity, 0));
        }
        else
        {
            int needToCraft = missing * 3;
            inventory.SetMaterial(new(lowerName, required.Type, required.Rarity, lowerAvailable - needToCraft));
            inventory.SetMaterial(new(materialName, required.Type, required.Rarity, 0));
            shortages.Add(new(materialName, required.Type, required.Rarity, 0));
        }
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

    private List<Material> TotalCost(Character character)
    {
        var charCost = this.characterUpgrade.GetCharacterCost(character);
        var skillCost = this.skillUpgrade.GetSkillsCost(character);
        return Merge(charCost, skillCost);
    }

    private string GetMaterialName(Character c, MaterialTypes type, MaterialRarity rarity) =>
    type switch
    {
        MaterialTypes.Gem => this.gems.GetMaterial(c, rarity),
        MaterialTypes.Book => this.books.GetMaterial(c, rarity),
        MaterialTypes.Enemy => this.enemies.GetMaterial(c, rarity),
        _ => throw new ArgumentException($"Unknown material type {type}"),
    };

    private void ConsumeCraftableMaterials(Inventory inventory, Material required, Character character, List<Material> shortages)
    {
        MaterialRarity[] chain = required.Type switch
        {
            MaterialTypes.Gem => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet, MaterialRarity.Orange],
            MaterialTypes.Book => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet],
            MaterialTypes.Enemy => [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue],
            _ => throw new ArgumentException($"Unsupported craftable type: {required.Type}"),
        };

        var materialsByTier = chain
            .ToDictionary(color => color, color => this.GetMaterialName(character, required.Type, color));

        MaterialRarity requiredColor = materialsByTier.First(x => x.Value == required.Name).Key;

        ConsumeWithCrafting(inventory, materialsByTier, requiredColor, required, shortages);
    }
}