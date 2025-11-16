using Newtonsoft.Json;

namespace Genshin_Calculator.Models
{
    public class Character
    {
        public Character(string name, Assets assets)
        {
            this.Name = name;
            this.Assets = assets;

            this.AutoAttack = new Skill();
            this.Elemental = new Skill();
            this.Burst = new Skill();
            this.Priority = ++Count;
        }

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
        public Assets? Assets { get; set; }

        private static int Count { get; set; }

        public Character Clone() => (Character)this.MemberwiseClone();
    }
}