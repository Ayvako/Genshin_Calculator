using Genshin_Calculator.Models.Enums;

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
            MaterialTypes.SkillMaterial => this.skillMaterials,
            MaterialTypes.Gem => this.gems,
            MaterialTypes.Enemy => this.enemies,
            MaterialTypes.Exp => this.exp,
            _ => null,
        };
    }
}