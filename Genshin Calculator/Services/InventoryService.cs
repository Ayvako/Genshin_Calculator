using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using Genshin_Calculator.Services.MaterialProviders;
using Genshin_Calculator.Services.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genshin_Calculator.Services;

public class InventoryService : IInventoryService
{
    private const string HeroWit = "HerosWit";

    private const int HeroWitXp = 20000;

    private const string AdventurerExperience = "AdventurersExperience";

    private const int AdventurerExperienceXp = 5000;

    private const string WandererAdvice = "WanderersAdvice";

    private const int WandererAdviceXp = 1000;

    private readonly IInventoryStore store;

    private readonly ISkillUpgradeService skillUpgrade;

    private readonly ICharacterUpgradeService characterUpgrade;

    private readonly IMaterialProviderFactory materialFactory;

    public InventoryService(
      IMaterialProviderFactory materialFactory,
      IInventoryStore store,
      ISkillUpgradeService skillUpgrade,
      ICharacterUpgradeService characterUpgrade)
    {
        this.skillUpgrade = skillUpgrade;
        this.store = store;
        this.materialFactory = materialFactory;
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

    public List<Material> GetRelatedMaterials(Character character, Material material)
    {
        IMaterialProvider? provider = this.materialFactory.GetProvider(material.Type);

        var inventory = this.GetInventory();

        if (provider == null)
        {
            return [inventory.GetMaterial(material.Name)];
        }

        var names = provider.GetMaterialGroup(character);

        return names.Select(inventory.GetMaterial).Where(m => m != null).ToList()!;
    }

    public Dictionary<Character, List<Material>> CalculateMissingMaterials(Inventory sourceInventory)
    {
        var tempInventory = sourceInventory.Clone();
        var result = new Dictionary<Character, List<Material>>();
        long totalExpPool = CalculateTotalExp(tempInventory);

        var activeCharacters = sourceInventory.NotDeletedCharacters
            .Where(c => c.Activated)
            .OrderBy(c => c.Priority)
            .ToList();

        foreach (var character in activeCharacters)
        {
            var requirements = this.TotalCost(character).Select(m => m.Clone()).ToList();

            DeductAvailableMaterials(requirements, tempInventory);
            this.ProcessMissingMaterials(character, requirements, tempInventory, ref totalExpPool);

            result[character] = this.BuildUiMaterials(character, requirements);
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
        MaterialTypes.WeeklyBoss => 5,
        MaterialTypes.MiniBoss => 6,
        MaterialTypes.LocalSpecialty => 4,
        MaterialTypes.Enemy => 2,
        MaterialTypes.SkillMaterial => 3,
        MaterialTypes.Mora => 99,
        MaterialTypes.Exp => 100,
        _ => 50,
    };

    private static long CalculateTotalExp(Inventory inv)
    {
        return ((long)(inv.GetMaterial(HeroWit)?.Amount ?? 0) * HeroWitXp)
          + ((long)(inv.GetMaterial(AdventurerExperience)?.Amount ?? 0) * AdventurerExperienceXp)
          + ((long)(inv.GetMaterial(WandererAdvice)?.Amount ?? 0) * WandererAdviceXp);
    }

    private static List<MaterialRarity> GetRarityChain(MaterialTypes type) => type switch
    {
        MaterialTypes.Gem => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet, MaterialRarity.Orange],
        MaterialTypes.SkillMaterial => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet],
        MaterialTypes.Enemy => [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue],
        _ => [],
    };

    private static void ConsumeExp(Material req, ref long totalExpPool)
    {
        long neededXp = (long)req.Amount * HeroWitXp;

        if (totalExpPool >= neededXp)
        {
            totalExpPool -= neededXp;
            req.Amount = 0;
        }
        else
        {
            long shortageXp = neededXp - totalExpPool;
            totalExpPool = 0;

            req.Amount = (int)Math.Ceiling((double)shortageXp / HeroWitXp);
        }
    }

    private static void DeductAvailableMaterials(List<Material> requirements, Inventory inventory)
    {
        foreach (var req in requirements)
        {
            if (req.Type == MaterialTypes.Exp)
            {
                continue;
            }

            var inStock = inventory.GetMaterial(req.Name);
            if (inStock != null && inStock.Amount > 0)
            {
                int taken = Math.Min(inStock.Amount, req.Amount);
                req.Amount -= taken;
                inventory.SetMaterial(new Material(inStock.Name, inStock.Type, inStock.Rarity, inStock.Amount - taken));
            }
        }
    }

    private void ProcessMissingMaterials(Character character, List<Material> requirements, Inventory inventory, ref long totalExpPool)
    {
        foreach (var req in requirements)
        {
            if (req.Amount <= 0)
            {
                continue;
            }

            if (req.Type == MaterialTypes.Exp)
            {
                ConsumeExp(req, ref totalExpPool);
                continue;
            }

            var chain = GetRarityChain(req.Type);
            if (chain.Contains(req.Rarity))
            {
                int tierIndex = chain.IndexOf(req.Rarity);
                req.Amount = this.ProcessTier(inventory, character, req.Type, chain, tierIndex, req.Amount);
            }
        }
    }

    private List<Material> BuildUiMaterials(Character character, List<Material> requirements)
    {
        var uiMaterials = this.TotalCost(character);
        var missingLookup = requirements.ToDictionary(m => m.Name, m => m.Amount);

        foreach (var uiMat in uiMaterials)
        {
            if (missingLookup.TryGetValue(uiMat.Name, out int stillNeeded) && stillNeeded > 0)
            {
                uiMat.IsCollected = false;
                uiMat.Amount = stillNeeded;
            }
            else
            {
                uiMat.IsCollected = true;
            }
        }

        return SortMaterialsForDisplay(uiMaterials);
    }

    private List<Material> TotalCost(Character character)
    {
        var charCost = this.characterUpgrade.GetCharacterCost(character);
        var skillCost = this.skillUpgrade.GetSkillsCost(character);

        var rawMaterials = charCost.Concat(skillCost);
        var resultList = new List<Material>();
        long totalXpAmount = 0;

        foreach (var group in rawMaterials.GroupBy(m => new { m.Name, m.Type, m.Rarity }))
        {
            if (group.Key.Type == MaterialTypes.Exp)
            {
                totalXpAmount += group.Sum(x => (long)x.Amount);
            }
            else
            {
                resultList.Add(new Material(group.Key.Name, group.Key.Type, group.Key.Rarity, group.Sum(x => x.Amount)));
            }
        }

        if (totalXpAmount > 0)
        {
            int heroWitCount = (int)Math.Ceiling((double)totalXpAmount / HeroWitXp);
            resultList.Add(new Material(HeroWit, MaterialTypes.Exp, MaterialRarity.Violet, heroWitCount));
        }

        return resultList;
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

        if (amountNeeded == 0 || tierIndex == 0)
        {
            return amountNeeded;
        }

        return this.CraftMissingItems(inventory, character, type, chain, tierIndex, amountNeeded);
    }

    private int CraftMissingItems(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int tierIndex, int amountNeeded)
    {
        while (amountNeeded > 0)
        {
            int[] cost = new int[tierIndex];

            if (this.TryGatherComponents(inventory, character, type, chain, tierIndex - 1, 3, cost))
            {
                this.ApplyCraftingCost(inventory, character, type, chain, cost);
                amountNeeded--;
            }
            else
            {
                break;
            }
        }

        return amountNeeded;
    }

    private bool TryGatherComponents(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int tIndex, int needed, int[] cost)
    {
        if (needed == 0)
        {
            return true;
        }

        if (tIndex < 0)
        {
            return false;
        }

        string tName = this.GetMaterialName(character, type, chain[tIndex]);
        int stock = inventory.GetMaterial(tName)?.Amount ?? 0;
        int avail = stock - cost[tIndex];

        int take = Math.Min(avail, needed);
        if (take > 0)
        {
            cost[tIndex] += take;
            needed -= take;
        }

        if (needed == 0)
        {
            return true;
        }

        if (tIndex > 0)
        {
            return this.TryGatherComponents(inventory, character, type, chain, tIndex - 1, needed * 3, cost);
        }

        return false;
    }

    private void ApplyCraftingCost(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int[] cost)
    {
        for (int i = 0; i < cost.Length; i++)
        {
            if (cost[i] <= 0)
            {
                continue;
            }

            string tName = this.GetMaterialName(character, type, chain[i]);
            var mat = inventory.GetMaterial(tName);

            if (mat != null)
            {
                inventory.SetMaterial(new Material(mat.Name, mat.Type, mat.Rarity, mat.Amount - cost[i]));
            }
        }
    }

    private string GetMaterialName(Character c, MaterialTypes type, MaterialRarity rarity)
    {
        var provider = this.materialFactory.GetProvider(type);

        return provider == null ? throw new ArgumentException($"No provider found for type {type}") : provider.GetMaterial(c, rarity);
    }
}