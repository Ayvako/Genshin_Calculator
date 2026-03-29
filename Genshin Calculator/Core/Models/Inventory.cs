using Genshin_Calculator.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Core.Models;

public class Inventory
{
    private Dictionary<string, Material> materialCache;

    public Inventory()
    {
        materialCache = [];
    }

    [JsonIgnore]
    public IEnumerable<Character> ActiveCharacters => Characters.Where(c => c.Activated && !c.Deleted);

    [JsonIgnore]
    public IEnumerable<Character> DeletedCharacters => Characters.Where(c => c.Deleted);

    [JsonIgnore]
    public IEnumerable<Character> NotDeletedCharacters => Characters.Where(c => !c.Deleted);

    public List<Material> Materials { get; set; } = [];

    public List<Character> Characters { get; set; } = [];

    public List<Character> GetAllCharacters()
    {
        return Characters;
    }

    public void AddMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (materialCache.TryGetValue(material.Name, out var existing))
        {
            existing.Amount += material.Amount;
        }
        else
        {
            var newMaterial = new Material(material.Name, material.Type, material.Rarity, material.Amount);
            Materials.Add(newMaterial);
            materialCache[material.Name] = newMaterial;
        }
    }

    public void SubtractMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (materialCache.TryGetValue(material.Name, out var existing))
        {
            existing.Amount -= material.Amount;

            if (existing.Amount < 0)
            {
                existing.Amount = 0;
            }
        }
        else
        {
            var newMaterial = new Material(material.Name, material.Type, material.Rarity, 0);
            Materials.Add(newMaterial);
            materialCache[material.Name] = newMaterial;
        }
    }

    public void SetMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (materialCache.TryGetValue(material.Name, out var existing))
        {
            existing.Amount = material.Amount;
        }
        else
        {
            var newMaterial = new Material(material.Name, material.Type, material.Rarity, material.Amount);
            Materials.Add(newMaterial);
            materialCache[material.Name] = newMaterial;
        }
    }

    public Material? GetMaterial(string name)
    {
        return materialCache.TryGetValue(name, out var material) ? material : null;
    }

    public void RefreshCache()
    {
        materialCache = Materials
            .GroupBy(m => m.Name)
            .Select(g => g.First())
            .ToDictionary(m => m.Name, m => m);
    }

    public Inventory Clone()
    {
        var clone = new Inventory
        {
            Characters = [.. Characters.Select(c => c.Clone())],
            Materials = [.. Materials.Select(m => m.Clone())],
        };
        clone.RefreshCache();
        return clone;
    }
}