using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Services;

public sealed record LevelMaterialData
(
    MaterialRarity SkillMaterialRarity,
    MaterialRarity EnemyRarity,
    int SkillMaterialAmount,
    int EnemyAmount,
    int MoraAmount,
    int WeeklyBossAmount = 0,
    int CrownAmount = 0);