using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Services.Interfaces;

namespace Genshin_Calculator.Services.MaterialProviders;

public class MaterialProviderFactory : IMaterialProviderFactory
{
    private readonly IMaterialProvider skillMaterials;

    private readonly IMaterialProvider gems;

    private readonly IMaterialProvider enemies;

    private readonly IMaterialProvider exp;

    public MaterialProviderFactory(SkillMaterialProvider skillMaterials, GemMaterialProvider gems, EnemyMaterialProvider enemies, ExpMaterialProvider exp)
    {
        this.skillMaterials = skillMaterials;
        this.gems = gems;
        this.enemies = enemies;
        this.exp = exp;
    }

    public IMaterialProvider? GetProvider(MaterialTypes materialType)
    {
        return materialType switch
        {
            MaterialTypes.SkillMaterial => skillMaterials,
            MaterialTypes.Gem => gems,
            MaterialTypes.Enemy => enemies,
            MaterialTypes.Exp => exp,
            _ => null,
        };
    }
}