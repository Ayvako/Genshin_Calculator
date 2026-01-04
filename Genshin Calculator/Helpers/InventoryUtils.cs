using System.Collections.Generic;
using System.Linq;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Helpers;

public static class InventoryUtils
{
    public static List<Material> Merge(params List<Material>[] dictionaries)
    {
        IEnumerable<Material> merged = dictionaries[0];
        for (int i = 1; i < dictionaries.Length; i++)
        {
            merged = merged.Concat(dictionaries[i]);
        }

        var groupedMaterials = merged.GroupBy(m => new { m.Name })
            .Select(g => new Material(g.Key.Name, g.First().Type, g.First().Rarity, g.Sum(m => m.Amount)))
            .ToList();

        return groupedMaterials;
    }
}