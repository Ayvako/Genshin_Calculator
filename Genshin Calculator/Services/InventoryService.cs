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

    public void Upgrade(Character character)
    {
        Inventory inventory = this.GetInventory();

        var missingMap = this.CalculateMissingMaterials(inventory);
        var missing = missingMap.TryGetValue(character, out List<MaterialRequirementUI>? value) ? value : [];

        bool canUpgrade = missing.All(m => m.IsCollected);

        if (canUpgrade)
        {
            long totalExpPool = CalculateTotalExp(inventory);
            var requirements = this.TotalCost(character).Select(m => m.Clone()).ToList();

            var dummyTracker = requirements.Select(m => new MaterialRequirementUI(m, m.Amount)).ToList();

            DeductAvailableMaterials(requirements, dummyTracker, inventory);
            this.ProcessMissingMaterials(character, requirements, dummyTracker, inventory, ref totalExpPool);

            character.CurrentLevel = character.DesiredLevel;
            character.AutoAttack.CurrentLevel = character.AutoAttack.DesiredLevel;
            character.Elemental.CurrentLevel = character.Elemental.DesiredLevel;
            character.Burst.CurrentLevel = character.Burst.DesiredLevel;

            Debug.WriteLine($"{character.Name}: Upgraded successfully.");
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

    public Dictionary<Character, List<MaterialRequirementUI>> CalculateMissingMaterials(Inventory sourceInventory)
    {
        var tempInventory = sourceInventory.Clone();
        var result = new Dictionary<Character, List<MaterialRequirementUI>>();
        long totalExpPool = CalculateTotalExp(tempInventory);

        var activeCharacters = sourceInventory.NotDeletedCharacters
            .Where(c => c.Activated)
            .OrderBy(c => c.Priority)
            .ToList();

        foreach (var character in activeCharacters)
        {
            var requirements = this.TotalCost(character).Select(m => m.Clone()).ToList();

            var uiTracker = requirements.Select(m => new MaterialRequirementUI(m.Clone(), m.Amount)).ToList();

            DeductAvailableMaterials(requirements, uiTracker, tempInventory);
            this.ProcessMissingMaterials(character, requirements, uiTracker, tempInventory, ref totalExpPool);

            result[character] = SortMaterialsForDisplay(uiTracker);
        }

        return result;
    }

    private static void ConsumeExp(Material req, Inventory inventory, ref long totalExpPool, MaterialRequirementUI uiMat)
    {
        long neededXp = (long)req.Amount * HeroWitXp;

        if (totalExpPool >= neededXp)
        {
            DeductExpFromInventory(neededXp, inventory, uiMat);
            totalExpPool -= neededXp;
            req.Amount = 0;
        }
        else
        {
            DeductExpFromInventory(totalExpPool, inventory, uiMat);
            long shortageXp = neededXp - totalExpPool;
            totalExpPool = 0;
            req.Amount = (int)Math.Ceiling((double)shortageXp / HeroWitXp);
        }
    }

    private static void DeductExpFromInventory(long xpToConsume, Inventory inventory, MaterialRequirementUI uiMat)
    {
        long remainingXp = xpToConsume;

        var heroWit = inventory.GetMaterial(HeroWit);
        if (heroWit != null && heroWit.Amount > 0 && remainingXp >= HeroWitXp)
        {
            int useCount = (int)Math.Min(heroWit.Amount, remainingXp / HeroWitXp);
            remainingXp -= (long)useCount * HeroWitXp;
            inventory.SetMaterial(new Material(HeroWit, MaterialTypes.Exp, MaterialRarity.Violet, heroWit.Amount - useCount));

            uiMat.TakenFromInventory += useCount;
        }

        var advExp = inventory.GetMaterial(AdventurerExperience);
        if (advExp != null && advExp.Amount > 0 && remainingXp >= AdventurerExperienceXp)
        {
            int useCount = (int)Math.Min(advExp.Amount, remainingXp / AdventurerExperienceXp);
            remainingXp -= (long)useCount * AdventurerExperienceXp;
            inventory.SetMaterial(new Material(AdventurerExperience, MaterialTypes.Exp, MaterialRarity.Blue, advExp.Amount - useCount));

            AddExpSubstituteCost(uiMat, AdventurerExperience, MaterialTypes.Exp, MaterialRarity.Blue, useCount);
        }

        var wandAdv = inventory.GetMaterial(WandererAdvice);
        if (wandAdv != null && wandAdv.Amount > 0 && remainingXp >= WandererAdviceXp)
        {
            int useCount = (int)Math.Min(wandAdv.Amount, remainingXp / WandererAdviceXp);
            remainingXp -= (long)useCount * WandererAdviceXp;
            inventory.SetMaterial(new Material(WandererAdvice, MaterialTypes.Exp, MaterialRarity.Green, wandAdv.Amount - useCount));

            AddExpSubstituteCost(uiMat, WandererAdvice, MaterialTypes.Exp, MaterialRarity.Green, useCount);
        }

        if (remainingXp > 0)
        {
            wandAdv = inventory.GetMaterial(WandererAdvice);
            if (wandAdv != null && wandAdv.Amount > 0)
            {
                inventory.SetMaterial(new Material(WandererAdvice, MaterialTypes.Exp, MaterialRarity.Green, wandAdv.Amount - 1));
                AddExpSubstituteCost(uiMat, WandererAdvice, MaterialTypes.Exp, MaterialRarity.Green, 1);
                remainingXp = 0;
            }
            else
            {
                advExp = inventory.GetMaterial(AdventurerExperience);
                if (advExp != null && advExp.Amount > 0)
                {
                    inventory.SetMaterial(new Material(AdventurerExperience, MaterialTypes.Exp, MaterialRarity.Blue, advExp.Amount - 1));
                    AddExpSubstituteCost(uiMat, AdventurerExperience, MaterialTypes.Exp, MaterialRarity.Blue, 1);
                    remainingXp = 0;
                }
                else
                {
                    heroWit = inventory.GetMaterial(HeroWit);
                    if (heroWit != null && heroWit.Amount > 0)
                    {
                        inventory.SetMaterial(new Material(HeroWit, MaterialTypes.Exp, MaterialRarity.Violet, heroWit.Amount - 1));
                        uiMat.TakenFromInventory += 1;
                        remainingXp = 0;
                    }
                }
            }
        }
    }

    private static void AddExpSubstituteCost(MaterialRequirementUI uiMat, string name, MaterialTypes type, MaterialRarity rarity, int amount)
    {
        var tracked = uiMat.AlchemyCosts.FirstOrDefault(m => m.Name == name);
        if (tracked != null)
        {
            tracked.Amount += amount;
        }
        else
        {
            uiMat.AlchemyCosts.Add(new Material(name, type, rarity, amount));
        }
    }

    private static void DeductAvailableMaterials(List<Material> requirements, List<MaterialRequirementUI> uiTracker, Inventory inventory)
    {
        foreach (var req in requirements)
        {
            if (req.Type == MaterialTypes.Exp)
            {
                continue;
            }

            var uiMat = uiTracker.First(u => u.TargetMaterial.Name == req.Name);
            var inStock = inventory.GetMaterial(req.Name);

            if (inStock != null && inStock.Amount > 0)
            {
                int taken = Math.Min(inStock.Amount, req.Amount);
                req.Amount -= taken;
                uiMat.TakenFromInventory += taken;
                inventory.SetMaterial(new Material(inStock.Name, inStock.Type, inStock.Rarity, inStock.Amount - taken));
            }
        }
    }

    private void ProcessMissingMaterials(Character character, List<Material> requirements, List<MaterialRequirementUI> uiTracker, Inventory inventory, ref long totalExpPool)
    {
        foreach (var req in requirements)
        {
            var uiMat = uiTracker.First(u => u.TargetMaterial.Name == req.Name);

            if (req.Amount <= 0)
            {
                uiMat.MissingAmount = 0;
                continue;
            }

            if (req.Type == MaterialTypes.Exp)
            {
                int neededBeforeConsume = req.Amount;

                ConsumeExp(req, inventory, ref totalExpPool, uiMat);

                int totalFulfilled = neededBeforeConsume - req.Amount;

                uiMat.CraftedAmount = totalFulfilled - uiMat.TakenFromInventory;
                uiMat.MissingAmount = req.Amount;
                continue;
            }

            var chain = GetRarityChain(req.Type);
            if (chain.Contains(req.Rarity))
            {
                int tierIndex = chain.IndexOf(req.Rarity);
                int neededBeforeCraft = req.Amount;

                req.Amount = this.ProcessTier(inventory, character, req.Type, chain, tierIndex, req.Amount, uiMat.AlchemyCosts);

                uiMat.CraftedAmount = neededBeforeCraft - req.Amount;
            }

            uiMat.MissingAmount = req.Amount;
        }
    }

    private int ProcessTier(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int tierIndex, int amountNeeded, List<Material> alchemyTracker)
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

        return this.CraftMissingItems(inventory, character, type, chain, tierIndex, amountNeeded, alchemyTracker);
    }

    private int CraftMissingItems(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int tierIndex, int amountNeeded, List<Material> alchemyTracker)
    {
        int crafted = 0;
        while (amountNeeded > 0)
        {
            int[] cost = new int[tierIndex];

            if (this.TryGatherComponents(inventory, character, type, chain, tierIndex - 1, 3, cost))
            {
                this.ApplyCraftingCost(inventory, character, type, chain, cost, alchemyTracker);
                amountNeeded--;
                crafted++;
            }
            else
            {
                break;
            }
        }

        return amountNeeded;
    }

    private void ApplyCraftingCost(Inventory inventory, Character character, MaterialTypes type, List<MaterialRarity> chain, int[] cost, List<Material> alchemyTracker)
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

                var tracked = alchemyTracker.FirstOrDefault(m => m.Name == tName);
                if (tracked != null)
                {
                    tracked.Amount += cost[i];
                }
                else
                {
                    alchemyTracker.Add(new Material(mat.Name, mat.Type, mat.Rarity, cost[i]));
                }
            }
        }
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

    private static List<MaterialRequirementUI> SortMaterialsForDisplay(List<MaterialRequirementUI> materials)
    {
        return [.. materials
        .OrderBy(m => GetTypePriority(m.TargetMaterial.Type))
        .ThenBy(m => m.TargetMaterial.Rarity)];
    }

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

    private string GetMaterialName(Character c, MaterialTypes type, MaterialRarity rarity)
    {
        var provider = this.materialFactory.GetProvider(type);

        return provider == null ? throw new ArgumentException($"No provider found for type {type}") : provider.GetMaterial(c, rarity);
    }
}