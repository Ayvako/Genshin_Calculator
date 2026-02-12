using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Models;

public partial class Material : ObservableObject
{
    [ObservableProperty]
    private int amount;

    public Material(string name, MaterialTypes type, MaterialRarity rarity, int amount)
    {
        this.Name = name;
        this.Type = type;
        this.Amount = amount;
        this.Rarity = rarity;
    }

    public string Name { get; set; }

    public MaterialTypes Type { get; set; }

    public MaterialRarity Rarity { get; set; }

    public Uri ImagePath => ResourcePaths.Material(this.Name);

    public override bool Equals(object? obj)
    {
        if (obj is not Material item)
        {
            return false;
        }

        return this.Name.Equals(item.Name);
    }

    public override int GetHashCode()
    {
        return this.Name.GetHashCode();
    }

    public Material Clone() => (Material)this.MemberwiseClone();
}