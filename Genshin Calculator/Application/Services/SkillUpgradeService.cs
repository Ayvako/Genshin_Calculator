using System.Collections.Generic;
using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Application.Services;

public class SkillUpgradeService : BaseUpgradeService, ISkillUpgradeService
{
    private readonly SkillLevelData skillData;

    public SkillUpgradeService(IMaterialProviderFactory factory, IEmbeddedDataRepository embeddedData)
        : base(factory)
    {
        this.skillData = embeddedData.GetSkillCosts();
    }

    public List<Material> GetSkillsCost(Character character)
    {
        var totalMaterials = new Dictionary<string, Material>();

        this.AddSkillCost(character, character.AutoAttack, totalMaterials);
        this.AddSkillCost(character, character.Elemental, totalMaterials);
        this.AddSkillCost(character, character.Burst, totalMaterials);

        return [.. totalMaterials.Values];
    }

    private void AddSkillCost(Character character, Skill skill, Dictionary<string, Material> total)
    {
        for (int i = skill.CurrentLevel + 1; i <= skill.DesiredLevel; i++)
        {
            if (this.skillData.LevelCosts.TryGetValue(i, out var templates))
            {
                foreach (var t in templates)
                {
                    MaterialMerger.AddToTotal(total, this.ResolveMaterial(character, t));
                }
            }
        }
    }
}