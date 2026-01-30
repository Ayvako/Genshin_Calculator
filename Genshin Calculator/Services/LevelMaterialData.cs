using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Services;

public sealed record LevelMaterialData
(
    MaterialRarity BookRarity,
    MaterialRarity EnemyRarity,
    int BookAmount,
    int EnemyAmount,
    int MoraAmount,
    int WeeklyBossAmount = 0,
    int CrownAmount = 0);