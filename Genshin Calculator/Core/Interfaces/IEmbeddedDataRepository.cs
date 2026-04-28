using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface IEmbeddedDataRepository
{
    LevelData GetLevelCosts();

    SkillLevelData GetSkillCosts();
}