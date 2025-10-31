namespace Genshin_Calculator.ProjectRoot.Src.Models
{
    public class Assets
    {
        public string Name { get; set; }

        public string LocalSpecialty { get; set; }

        public string Element { get; set; }

        public string Weapon { get; set; }

        public string Enemy { get; set; }

        public string MiniBoss { get; set; }

        public string WeeklyBoss { get; set; }

        public string BookType { get; set; }

        public int Rarity { get; set; }

        public Assets(string name, string weapon, string element, string localSpecialty, string bookType, string enemy, string miniBoss, string weeklyBoss, int rarity)
        {
            this.Name = name;
            this.LocalSpecialty = localSpecialty;
            this.BookType = bookType;
            this.Element = element;
            this.Weapon = weapon;
            this.Enemy = enemy;
            this.MiniBoss = miniBoss;
            this.WeeklyBoss = weeklyBoss;
            this.Rarity = rarity;
        }

        public override string ToString()
        {
            return $"[{this.LocalSpecialty}, {this.BookType}, {this.Element}, {this.Weapon}, {this.Enemy}, {this.MiniBoss}, {this.WeeklyBoss}]";
        }
    }
}