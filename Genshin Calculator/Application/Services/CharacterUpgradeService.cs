using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using System.Collections.Generic;

namespace Genshin_Calculator.Application.Services;

public class CharacterUpgradeService : BaseUpgradeService, ICharacterUpgradeService
{
    private readonly LevelData levelData;

    public CharacterUpgradeService(IMaterialProviderFactory factory, IEmbeddedDataRepository embeddedData)
        : base(factory)
    {
        this.levelData = embeddedData.GetLevelCosts();
    }

    public List<Material> GetCharacterCost(Character character)
    {
        var total = new Dictionary<string, Material>(16);

        var levels = LevelHelper.GetRange(
            character.CurrentLevel,
            character.DesiredLevel);

        foreach (var level in levels)
        {
            this.ProcessLevel(level, character, total);
        }

        return [.. total.Values];
    }

    private void ProcessLevel(Level level, Character character, Dictionary<string, Material> total)
    {
        this.AddBaseCost(level, total);
        this.AddAscensionCost(level, character, total);
    }

    private void AddBaseCost(Level level, Dictionary<string, Material> total)
    {
        if (this.levelData.BaseCosts.TryGetValue(level.ToString(), out int exp))
        {
            MaterialMerger.AddToTotal(
                total,
                new Material(ItemIds.WanderersAdvice, MaterialTypes.Exp, MaterialRarity.Green, exp));

            MaterialMerger.AddToTotal(
                total,
                new Material(ItemIds.Mora, MaterialTypes.Mora, MaterialRarity.Blue, exp / 5));
        }
    }

    private void AddAscensionCost(Level level, Character character, Dictionary<string, Material> total)
    {
        if (!this.levelData.AscensionCosts.TryGetValue(level.ToString(), out var templates))
        {
            return;
        }

        foreach (var t in templates)
        {
            MaterialMerger.AddToTotal(total, this.ResolveMaterial(character, t));
        }
    }
}