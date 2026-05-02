using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Application.Services;

public abstract class BaseUpgradeService
{
    private readonly IMaterialProviderFactory providerFactory;

    protected BaseUpgradeService(IMaterialProviderFactory providerFactory)
    {
        this.providerFactory = providerFactory;
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
            MaterialTypes.StellaFortuna => ItemIds.StellaFortuna,
            MaterialTypes.Crown => ItemIds.CrownOfInsight,
            MaterialTypes.Mora => ItemIds.Mora,
            _ => "Unknown",
        };

        return new Material(name, template.Type, template.Rarity, template.Amount);
    }
}