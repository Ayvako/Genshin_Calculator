using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface ITalentLevelRules
{
    int GetMaxTalentLevel(Level level);

    Level GetRequiredLevel(int talentLevel);

    int ClampTalentLevel(Level level, int talentLevel);
}