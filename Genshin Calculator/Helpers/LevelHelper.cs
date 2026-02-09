using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Genshin_Calculator.Helpers;

public static class LevelHelper
{
    public static readonly ImmutableArray<string> Levels =
    [
        "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
        "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "20★",
        "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "40★",
        "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "50★",
        "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "60★",
        "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "70★",
        "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "80★",
        "81", "82", "83", "84", "85", "86", "87", "88", "89", "90",
    ];

    private static readonly Dictionary<string, int> AscensionTalentLimits = new()
    {
        { "20", 1 },
        { "20★", 1 },
        { "40★", 2 },
        { "50★", 4 },
        { "60★", 6 },
        { "70★", 8 },
        { "80★", 10 },
        { "90", 10 },
    };

    public static int LevelToIndex(string level)
    {
        return Levels.IndexOf(level);
    }

    public static int CompareLevels(string a, string b)
    {
        return LevelToIndex(a).CompareTo(LevelToIndex(b));
    }

    public static int GetMaxTalentLevel(string characterLevel)
    {
        int currentNumericLevel = int.Parse(characterLevel.Replace("★", string.Empty));
        bool hasStar = characterLevel.Contains('★');

        var possibleLimits = AscensionTalentLimits
            .Select(x => new
            {
                Level = int.Parse(x.Key.Replace("★", string.Empty)),
                HasStar = x.Key.Contains('★'),
                Limit = x.Value,
            })
            .Where(x => x.Level <= currentNumericLevel)
            .OrderByDescending(x => x.Level)
            .ThenByDescending(x => x.HasStar);
        foreach (var item in possibleLimits)
        {
            if (item.Level < currentNumericLevel)
            {
                return item.Limit;
            }

            if (item.Level == currentNumericLevel && (hasStar || !item.HasStar))
            {
                return item.Limit;
            }
        }

        return 1;
    }

    public static string GetRequiredLevelForTalent(int talentLevel)
    {
        var requirement = AscensionTalentLimits
            .Select(x => new { x.Key, Limit = x.Value, Level = int.Parse(x.Key.Replace("★", string.Empty)) })
            .OrderBy(x => x.Level)
            .ThenBy(x => x.Key.Contains('★'))
            .FirstOrDefault(x => x.Limit >= talentLevel);

        return requirement?.Key ?? "80★";
    }
}