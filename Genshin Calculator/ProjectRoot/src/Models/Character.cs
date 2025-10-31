using Newtonsoft.Json;

namespace Genshin_Calculator.ProjectRoot.Src.Models
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

        public bool Activated { get; set; }

        public int Priority { get; set; }

        [JsonIgnore]
        public Assets Assets { get; set; }

        private static int Count { get; set; }

        public Character(string name, Assets assets)
        {
            this.Name = name;
            this.Assets = assets;

            this.AutoAttack = new Skill();
            this.Elemental = new Skill();
            this.Burst = new Skill();
            this.Priority = ++Count;
        }

        public static void AddCharacter(Character character) => character.Deleted = false;

        public static void CharacrerLevelUp(Character character, string to) => character.DesiredLevel = to;

        public static void AAChangeLevel(Character character, int to) => character.AutoAttack.DesiredLevel = to;

        public static void ElemChangeLevel(Character character, int to) => character.Elemental.DesiredLevel = to;

        public static void BurstChangeLevel(Character character, int to) => character.Burst.DesiredLevel = to;

        public override string ToString()
        {
            return $"Name: {this.Name,-25} Assets: {this.Assets,-50}";
        }

        public Character Clone() => (Character)this.MemberwiseClone();
    }
}