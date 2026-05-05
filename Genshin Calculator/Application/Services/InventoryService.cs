using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Application.State;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Application.Services;

internal sealed class InventoryService : IInventoryService
{
    private readonly InventoryStore store;

    private readonly ISkillUpgradeService skillUpgrade;

    private readonly ICharacterUpgradeService characterUpgrade;

    private readonly IMaterialProviderFactory materialFactory;

    private readonly IExperienceService experienceService;

    private readonly IAlchemyService alchemyService;

    public InventoryService(
          IMaterialProviderFactory materialFactory,
          InventoryStore store,
          ISkillUpgradeService skillUpgrade,
          ICharacterUpgradeService characterUpgrade,
          IExperienceService experienceService,
          IAlchemyService alchemyService)
    {
        this.skillUpgrade = skillUpgrade;
        this.store = store;
        this.materialFactory = materialFactory;
        this.characterUpgrade = characterUpgrade;
        this.experienceService = experienceService;
        this.alchemyService = alchemyService;
    }

    public static List<MaterialRequirement> SortMaterialsForDisplay(List<MaterialRequirement> materials)
    {
        return [.. materials
        .OrderBy(m => GetTypePriority(m.TargetMaterial.Type))
        .ThenBy(m => m.TargetMaterial.Rarity)];
    }

    public void Upgrade(Character character)
    {
        Inventory realInventory = this.GetInventory();
        long realExpPool = this.experienceService.CalculateTotalExp(realInventory);
        this.GetRequirementsForCharacter(character, realInventory, ref realExpPool);

        character.CurrentLevel = character.DesiredLevel;
        character.AutoAttack.CurrentLevel = character.AutoAttack.DesiredLevel;
        character.Elemental.CurrentLevel = character.Elemental.DesiredLevel;
        character.Burst.CurrentLevel = character.Burst.DesiredLevel;
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

    public Dictionary<Character, List<MaterialRequirement>> CalculateMissingMaterials(Inventory sourceInventory)
    {
        var tempInventory = sourceInventory.Clone();
        var result = new Dictionary<Character, List<MaterialRequirement>>();
        long totalExpPool = this.experienceService.CalculateTotalExp(tempInventory);

        var activeCharacters = sourceInventory.NotDeletedCharacters
            .Where(c => c.Activated)
            .OrderBy(c => c.Priority)
            .ToList();

        foreach (var character in activeCharacters)
        {
            var uiTracker = this.GetRequirementsForCharacter(character, tempInventory, ref totalExpPool);
            result[character] = SortMaterialsForDisplay(uiTracker);
        }

        return result;
    }

    public List<Material> TotalCost(Character character)
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
            resultList.Add(this.experienceService.ConvertXpToHeroWit(totalXpAmount));
        }

        return resultList;
    }

    private static void DeductAvailableMaterials(List<Material> requirements, List<MaterialRequirement> uiTracker, Inventory inventory)
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

    private void ProcessMissingMaterials(Character character, List<Material> requirements, List<MaterialRequirement> uiTracker, Inventory inventory, ref long totalExpPool)
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
                this.experienceService.ProcessExpRequirement(req, inventory, ref totalExpPool, uiMat);
                uiMat.CraftedAmount = neededBeforeConsume - req.Amount - uiMat.TakenFromInventory;
                uiMat.MissingAmount = req.Amount;
                continue;
            }

            if (this.alchemyService.IsCraftable(req.Type))
            {
                int neededBeforeCraft = req.Amount;
                req.Amount = this.alchemyService.ProcessCrafting(inventory, character, req.Type, req.Rarity, req.Amount, uiMat.AlchemyCosts);
                uiMat.CraftedAmount = neededBeforeCraft - req.Amount;
            }

            uiMat.MissingAmount = req.Amount;
        }
    }

    private List<MaterialRequirement> GetRequirementsForCharacter(Character character, Inventory inventory, ref long totalExpPool)
    {
        var requirements = this.TotalCost(character).Select(m => m.Clone()).ToList();
        var uiTracker = requirements.Select(m => new MaterialRequirement(m.Clone(), m.Amount)).ToList();

        DeductAvailableMaterials(requirements, uiTracker, inventory);
        this.ProcessMissingMaterials(character, requirements, uiTracker, inventory, ref totalExpPool);

        return uiTracker;
    }
}