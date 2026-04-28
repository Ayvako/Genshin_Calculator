using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

internal static class MaterialMerger
{
    public static void AddToTotal(Dictionary<string, Material> total, Material mat)
    {
        if (total.TryGetValue(mat.Name, out var existing))
        {
            existing.Amount += mat.Amount;
        }
        else
        {
            total[mat.Name] = new Material(mat.Name, mat.Type, mat.Rarity, mat.Amount);
        }
    }
}