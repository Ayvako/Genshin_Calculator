using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Services.Materials;

public class MaterialProviderFactory : IMaterialProviderFactory
{
    private readonly IMaterialProvider skillMaterials;

    private readonly IMaterialProvider gems;

    private readonly IMaterialProvider enemies;

    public MaterialProviderFactory(SkillMaterialProvider skillMaterials, GemMaterialProvider gems, EnemyMaterialProvider enemies)
    {
        this.skillMaterials = skillMaterials;
        this.gems = gems;
        this.enemies = enemies;
    }

    public IMaterialProvider? GetProvider(MaterialTypes materialType)
    {
        return materialType switch
        {
            MaterialTypes.SkillMaterial => this.skillMaterials,
            MaterialTypes.Gem => this.gems,
            MaterialTypes.Enemy => this.enemies,
            _ => null,
        };
    }
}