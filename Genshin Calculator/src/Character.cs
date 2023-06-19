using Newtonsoft.Json;

namespace Genshin.src
{
    public class Character
    {

        public string Name { get; set; }
        public string CurrentLevel { get; set; } = "1";
        public string DesiredLevel { get; set; } = "1";
        public Skill AutoAttack { get; set; }
        public Skill Elemental { get; set; }
        public Skill Burst { get; set; }
        public bool Deleted { get; set; } = true;
        public bool Activated { get; set; } = false;
        public int Priority { get; set; }
        [JsonIgnore]
        public Assets Assets { get; set; }

        private static int Count { get; set; }


        public Character(string name ,Assets assets)
        {

            Name = name;
            Assets = assets;

            AutoAttack = new Skill();
            Elemental = new Skill();
            Burst = new Skill();
            Priority = ++Count;
        }

        public override string ToString()
        {
            return $"Name: {Name,-25} Assets: {Assets,-50}";
        }

    }

    public class Skill
    {
        public int CurrentLevel { get; set; }
        public int DesiredLevel { get; set; }
        public Skill(int currentLevel = 1, int desiredLevel = 1)
        {
            CurrentLevel = currentLevel;
            DesiredLevel = desiredLevel;
        }
    }

    public class Assets
    {
        public string Name { get; set; }

        public string LocalSpecialty { get;  set; }
        public string Element { get; set; }
        public string Weapon { get; set; }
        public string Enemy { get; set; }
        public string MiniBoss { get; set; }
        public string WeeklyBoss { get; set; }
        public string BookType { get; set; }

        public Assets(string name, string weapon, string element, string localSpecialty, string bookType, string enemy, string miniBoss, string weeklyBoss)
        {
            Name = name;
            LocalSpecialty = localSpecialty;
            BookType = bookType;
            Element = element;
            Weapon = weapon;
            Enemy = enemy;
            MiniBoss = miniBoss;
            WeeklyBoss = weeklyBoss;

        }
        public override string ToString()
        {
            return $"[{LocalSpecialty}, {BookType}, {Element}, {Weapon}, {Enemy}, {MiniBoss}, {WeeklyBoss}]";
        }
    }

    public class Element
    {
        public const string ANEMO   = "Anemo";
        public const string HYDRO   = "Hydro";
        public const string GEO     = "Geo";
        public const string PYRO    = "Pyro";
        public const string CRYO    = "Cryo";
        public const string ELECTRO = "Electro";
        public const string DENDRO  = "Dendro";
    }
    public class Weapon
    {
        public const string BOW      = "Bow";
        public const string SWORD    = "Sword";
        public const string CLAYMORE = "Claymore";
        public const string CATALYST = "Catalyst";
        public const string POLEARM  = "Polearm";
    }
}