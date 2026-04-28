using System.Collections.Generic;
using System.Linq;
using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Application.Services;

public class CharacterUpgradeService : BaseUpgradeService, ICharacterUpgradeService
{
    private static readonly List<string> Levels = [.. LevelHelper.Levels];

    private readonly LevelData levelData;

    public CharacterUpgradeService(IMaterialProviderFactory factory, IEmbeddedDataRepository embeddedData)
        : base(factory)
    {
        this.levelData = embeddedData.GetLevelCosts();
    }

    public List<Material> GetCharacterCost(Character character)
    {
        var totalMaterials = new Dictionary<string, Material>();

        int startIndex = Levels.FindIndex(s => s.Contains(character.CurrentLevel));
        int endIndex = Levels.FindIndex(s => s.Contains(character.DesiredLevel));

        if (startIndex == -1 || endIndex == -1)
        {
            return [];
        }

        var levelsInRange = Levels.Skip(startIndex + 1).Take(endIndex - startIndex);

        foreach (var level in levelsInRange)
        {
            if (this.levelData.BaseCosts.TryGetValue(level, out int expAmount))
            {
                MaterialMerger.AddToTotal(totalMaterials, new Material("WanderersAdvice", MaterialTypes.Exp, MaterialRarity.Green, expAmount));
                MaterialMerger.AddToTotal(totalMaterials, new Material("Mora", MaterialTypes.Mora, MaterialRarity.Blue, expAmount / 5));
            }

            if (this.levelData.AscensionCosts.TryGetValue(level, out var templates))
            {
                foreach (var t in templates)
                {
                    MaterialMerger.AddToTotal(totalMaterials, this.ResolveMaterial(character, t));
                }
            }
        }

        return [.. totalMaterials.Values];
    }
}