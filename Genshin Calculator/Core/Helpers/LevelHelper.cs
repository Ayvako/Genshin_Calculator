using Genshin_Calculator.Core.Models;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Genshin_Calculator.Core.Helpers;

public static class LevelHelper
{
    public static readonly ImmutableList<Level> Levels = Build();

    public static IEnumerable<Level> GetRange(Level from, Level to)
    {
        int start = Levels.IndexOf(from);
        int end = Levels.IndexOf(to);

        if (start == -1 || end == -1 || start >= end)
        {
            yield break;
        }

        for (int i = start + 1; i <= end; i++)
        {
            yield return Levels[i];
        }
    }

    private static ImmutableList<Level> Build()
    {
        var list = new List<Level>();

        void AddRange(int from, int to, bool ascendedAtEnd = false)
        {
            for (int i = from; i < to; i++)
            {
                list.Add(new Level(i, false));
            }

            list.Add(new Level(to, false));

            if (ascendedAtEnd)
            {
                list.Add(new Level(to, true));
            }
        }

        AddRange(1, 20, true);
        AddRange(21, 40, true);
        AddRange(41, 50, true);
        AddRange(51, 60, true);
        AddRange(61, 70, true);
        AddRange(71, 80, true);

        for (int i = 81; i <= 90; i++)
        {
            list.Add(new Level(i, false));
        }

        list.Add(new Level(95, false));
        list.Add(new Level(100, false));

        return [.. list];
    }
}