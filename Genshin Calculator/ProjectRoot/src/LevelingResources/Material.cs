namespace Genshin_Calculator.ProjectRoot.src.LevelingResources;

public class Material
{
    public string Name { get; set; }

    public int Amount { get; set; }

    public string Type { get; set; }

    public int Rarity { get; set; }

    public Material(string name, string type, int rarity, int amount)
    {
        this.Name = name;
        this.Type = type;
        this.Amount = amount;
        this.Rarity = rarity;
    }

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

    public override string ToString()
    {
        return $"Material: {this.Name,-25} Amount: {this.Amount,-10} Type: {this.Type}";
    }

    public Material Clone() => (Material)this.MemberwiseClone();
}
