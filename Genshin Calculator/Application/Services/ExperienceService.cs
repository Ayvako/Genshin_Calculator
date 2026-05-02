using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using System;
using System.Linq;

namespace Genshin_Calculator.Application.Services;

public class ExperienceService : IExperienceService
{
    private const int HeroWitXp = 20000;

    private const int AdventurerExperienceXp = 5000;

    private const int WandererAdviceXp = 1000;

    public long CalculateTotalExp(Inventory inventory)
    {
        return ((long)(inventory.GetMaterial(ItemIds.HerosWit)?.Amount ?? 0) * HeroWitXp)
             + ((long)(inventory.GetMaterial(ItemIds.AdventurersExperience)?.Amount ?? 0) * AdventurerExperienceXp)
             + ((long)(inventory.GetMaterial(ItemIds.WanderersAdvice)?.Amount ?? 0) * WandererAdviceXp);
    }

    public void ProcessExpRequirement(Material req, Inventory inventory, ref long totalExpPool, MaterialRequirement uiMat)
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

    public Material ConvertXpToHeroWit(long totalXpAmount)
    {
        int heroWitCount = (int)Math.Ceiling((double)totalXpAmount / HeroWitXp);
        return new Material(ItemIds.HerosWit, MaterialTypes.Exp, MaterialRarity.Violet, heroWitCount);
    }

    private static void DeductExpFromInventory(long xpToConsume, Inventory inventory, MaterialRequirement uiMat)
    {
        long remainingXp = xpToConsume;

        remainingXp = ConsumeMaterialByXp(inventory, uiMat, ItemIds.HerosWit, HeroWitXp, MaterialTypes.Exp, MaterialRarity.Violet, remainingXp, incrementTakenFromInventory: true);
        remainingXp = ConsumeMaterialByXp(inventory, uiMat, ItemIds.AdventurersExperience, AdventurerExperienceXp, MaterialTypes.Exp, MaterialRarity.Blue, remainingXp, incrementTakenFromInventory: false);
        remainingXp = ConsumeMaterialByXp(inventory, uiMat, ItemIds.WanderersAdvice, WandererAdviceXp, MaterialTypes.Exp, MaterialRarity.Green, remainingXp, incrementTakenFromInventory: false);

        if (remainingXp > 0)
        {
            ConsumeSingleFallback(inventory, uiMat);
        }
    }

    private static long ConsumeMaterialByXp(Inventory inventory, MaterialRequirement uiMat, string name, int xpValue, MaterialTypes type, MaterialRarity rarity, long remainingXp, bool incrementTakenFromInventory)
    {
        var mat = inventory.GetMaterial(name);
        if (mat == null || mat.Amount <= 0 || remainingXp < xpValue)
        {
            return remainingXp;
        }

        int useCount = (int)Math.Min(mat.Amount, remainingXp / xpValue);
        remainingXp -= (long)useCount * xpValue;
        inventory.SetMaterial(new Material(name, type, rarity, mat.Amount - useCount));

        if (incrementTakenFromInventory)
        {
            uiMat.TakenFromInventory += useCount;
        }
        else
        {
            AddExpSubstituteCost(uiMat, name, type, rarity, useCount);
        }

        return remainingXp;
    }

    private static void ConsumeSingleFallback(Inventory inventory, MaterialRequirement uiMat)
    {
        var wand = inventory.GetMaterial(ItemIds.WanderersAdvice);
        if (wand != null && wand.Amount > 0)
        {
            inventory.SetMaterial(new Material(ItemIds.WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, wand.Amount - 1));
            AddExpSubstituteCost(uiMat, ItemIds.WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, 1);
            return;
        }

        var adv = inventory.GetMaterial(ItemIds.AdventurersExperience);
        if (adv != null && adv.Amount > 0)
        {
            inventory.SetMaterial(new Material(ItemIds.AdventurersExperience, MaterialTypes.Exp, MaterialRarity.Blue, adv.Amount - 1));
            AddExpSubstituteCost(uiMat, ItemIds.AdventurersExperience, MaterialTypes.Exp, MaterialRarity.Blue, 1);
            return;
        }

        var hero = inventory.GetMaterial(ItemIds.HerosWit);
        if (hero != null && hero.Amount > 0)
        {
            inventory.SetMaterial(new Material(ItemIds.HerosWit, MaterialTypes.Exp, MaterialRarity.Violet, hero.Amount - 1));
            uiMat.TakenFromInventory += 1;
        }
    }

    private static void AddExpSubstituteCost(MaterialRequirement uiMat, string name, MaterialTypes type, MaterialRarity rarity, int amount)
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
}