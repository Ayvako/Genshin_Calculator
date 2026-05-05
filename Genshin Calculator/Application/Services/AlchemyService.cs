using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Application.Services;

public class AlchemyService : IAlchemyService
{
    private readonly IMaterialProviderFactory materialFactory;

    public AlchemyService(IMaterialProviderFactory materialFactory)
    {
        this.materialFactory = materialFactory;
    }

    public bool IsCraftable(MaterialTypes type)
    {
        return GetRarityChain(type).Count > 0;
    }

    public int ProcessCrafting(Inventory inventory, Character character, MaterialTypes type, MaterialRarity targetRarity, int amountNeeded, List<Material> alchemyTracker)
    {
        var chain = GetRarityChain(type);
        if (!chain.Contains(targetRarity))
        {
            return amountNeeded;
        }

        int tierIndex = chain.IndexOf(targetRarity);
        return this.ProcessTier(inventory, character, type, chain, tierIndex, amountNeeded, alchemyTracker);
    }

    private static List<MaterialRarity> GetRarityChain(MaterialTypes type) => type switch
    {
        MaterialTypes.Gem => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet, MaterialRarity.Orange],
        MaterialTypes.SkillMaterial => [MaterialRarity.Green, MaterialRarity.Blue, MaterialRarity.Violet],
        MaterialTypes.Enemy => [MaterialRarity.White, MaterialRarity.Green, MaterialRarity.Blue],
        _ => [],

    };

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
        while (amountNeeded > 0)
        {
            int[] cost = new int[tierIndex];
            if (this.TryGatherComponents(inventory, character, type, chain, tierIndex - 1, 3, cost))
            {
                this.ApplyCraftingCost(inventory, character, type, chain, cost, alchemyTracker);
                amountNeeded--;
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

    private string GetMaterialName(Character c, MaterialTypes type, MaterialRarity rarity)
    {
        var provider = this.materialFactory.GetProvider(type);
        return provider == null ? throw new ArgumentException($"No provider found for type {type}") : provider.GetMaterial(c, rarity);
    }
}