using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Core.Services;

public class DefaultTalentLevelRules : ITalentLevelRules
{
    private readonly List<(Level Level, int Limit)> rules =
    [
        (new Level(20, false), 1),
        (new Level(20, true), 1),
        (new Level(40, true), 2),
        (new Level(50, true), 4),
        (new Level(60, true), 6),
        (new Level(70, true), 8),
        (new Level(80, true), 10),
        (new Level(90, false), 10),
    ];

    public int GetMaxTalentLevel(Level level)
    {
        var result = this.rules
            .Where(r => IsApplicable(r.Level, level))
            .OrderByDescending(r => r.Level.Value)
            .ThenByDescending(r => r.Level.IsAscended)
            .FirstOrDefault();

        return result == default ? 1 : result.Limit;
    }

    public Level GetRequiredLevel(int talentLevel)
    {
        if (talentLevel <= 1)
        {
            return new Level(1, false);
        }

        return this.rules
            .OrderBy(r => r.Level.Value)
            .ThenBy(r => r.Level.IsAscended)
            .First(r => r.Limit >= talentLevel)
            .Level;
    }

    public int ClampTalentLevel(Level level, int talentLevel)
    => Math.Min(talentLevel, this.GetMaxTalentLevel(level));

    private static bool IsApplicable(Level rule, Level current)
    {
        if (rule.Value < current.Value)
        {
            return true;
        }

        if (rule.Value == current.Value)
        {
            return current.IsAscended || !rule.IsAscended;
        }

        return false;
    }
}