using Genshin_Calculator.Core.Models.Enums;
using Newtonsoft.Json;

namespace Genshin_Calculator.Core.Models;

public class Material
{
    public Material(string name, MaterialTypes type, MaterialRarity rarity, int amount)
    {
        this.Name = name;
        this.Type = type;
        this.Amount = amount;
        this.Rarity = rarity;
    }

    public string Name { get; set; }

    [JsonIgnore]
    public MaterialTypes Type { get; set; }

    [JsonIgnore]
    public MaterialRarity Rarity { get; set; }

    public int Amount { get; set; }

    public Material Clone() => (Material)this.MemberwiseClone();

    public override bool Equals(object? obj) => obj is Material m && this.Name.Equals(m.Name);

    public override int GetHashCode() => this.Name.GetHashCode();
}