using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.MaterialProviders;
using System.Collections.Generic;

namespace Genshin_Calculator.Services;

public abstract class BaseUpgradeService
{
    private readonly IMaterialProviderFactory providerFactory;

    protected BaseUpgradeService(IMaterialProviderFactory providerFactory)
    {
        this.providerFactory = providerFactory;
    }

    protected static void AddToTotal(Dictionary<string, Material> total, Material mat)
    {
        if (total.TryGetValue(mat.Name, out var existing))
        {
            existing.Amount += mat.Amount;
        }
        else
        {
            total[mat.Name] = new Material(mat.Name, mat.Type, mat.Rarity, mat.Amount);
        }
    }

    protected Material ResolveMaterial(Character character, TemplateItem template)
    {
        string name = template.Type switch
        {
            MaterialTypes.Gem or MaterialTypes.Enemy or MaterialTypes.SkillMaterial or MaterialTypes.Exp
                => this.providerFactory.GetProvider(template.Type)?.GetMaterial(character, template.Rarity) ?? "Unknown",

            MaterialTypes.LocalSpecialty => character.Assets?.LocalSpecialty ?? "Unknown",
            MaterialTypes.MiniBoss => character.Assets?.MiniBoss ?? "Unknown",
            MaterialTypes.WeeklyBoss => character.Assets?.WeeklyBoss ?? "Unknown Boss",

            MaterialTypes.Crown => "CrownOfInsight",
            MaterialTypes.Mora => "Mora",
            _ => "Unknown",
        };

        return new Material(name, template.Type, template.Rarity, template.Amount);
    }
}