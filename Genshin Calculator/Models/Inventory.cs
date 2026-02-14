using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace Genshin_Calculator.Models;

public class Inventory
{
    private Dictionary<string, Material> materialCache;

    public Inventory()
    {
        this.materialCache = [];
    }

    [JsonIgnore]
    public IEnumerable<Character> ActiveCharacters => this.Characters.Where(c => c.Activated && !c.Deleted);

    [JsonIgnore]
    public IEnumerable<Character> DeletedCharacters => this.Characters.Where(c => c.Deleted);

    [JsonIgnore]
    public IEnumerable<Character> NotDeletedCharacters => this.Characters.Where(c => !c.Deleted);

    public List<Material> Materials { get; set; } = [];

    public List<Character> Characters { get; set; } = [];

    public List<Character> GetAllCharacters()
    {
        return this.Characters;
    }

    public void AddMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (this.materialCache.TryGetValue(material.Name, out var existing))
        {
            existing.Amount += material.Amount;
        }
        else
        {
            var newMaterial = new Material(material.Name, material.Type, material.Rarity, material.Amount);
            this.Materials.Add(newMaterial);
            this.materialCache[material.Name] = newMaterial;
        }
    }

    public void SubtractMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (this.materialCache.TryGetValue(material.Name, out var existing))
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
            this.Materials.Add(newMaterial);
            this.materialCache[material.Name] = newMaterial;
        }
    }

    public void SetMaterial(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (this.materialCache.TryGetValue(material.Name, out var existing))
        {
            existing.Amount = material.Amount;
        }
        else
        {
            var newMaterial = new Material(material.Name, material.Type, material.Rarity, material.Amount);
            this.Materials.Add(newMaterial);
            this.materialCache[material.Name] = newMaterial;
        }
    }

    public Material? GetMaterial(string name)
    {
        return this.materialCache.TryGetValue(name, out var material) ? material : null;
    }

    public void RefreshCache()
    {
        this.materialCache = this.Materials
            .GroupBy(m => m.Name)
            .Select(g => g.First())
            .ToDictionary(m => m.Name, m => m);
    }

    public Inventory Clone()
    {
        var clone = new Inventory
        {
            Characters = [.. this.Characters.Select(c => c.Clone())],
            Materials = [.. this.Materials.Select(m => m.Clone())],
        };
        clone.RefreshCache();
        return clone;
    }
}