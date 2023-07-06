namespace Genshin.src.LevelingResources
{
    public class Material
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Type { get; set; }
        public int Rarity { get; set; }

        public Material(string name, string type, int rarity, int amount)
        {
            Name = name;
            Type = type;
            Amount = amount;
            Rarity = rarity;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not Material item)
            {
                return false;
            }

            return Name.Equals(item.Name);

        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override string ToString()
        {
            return $"Material: {Name,-25} Amount: {Amount, -10} Type: {Type}";
        }

    }

    static class MaterailTypes
    {
        public const string GEM   = "Gem";
        public const string ENEMY = "Enemy";
        public const string BOOK  = "Book";
        public const string EXP   = "Exp";
        public const string OTHER = "Other";
    }


}
