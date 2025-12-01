using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Models
{
    public class Assets
    {
        public Assets(string name, string weapon, string element, string localSpecialty, string bookType, string enemy, string miniBoss, string weeklyBoss, MaterialRarity rarity)
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

        public string Name { get; set; }

        public string ImagePath => $"{App.Configuration["Resources:CharactersBasePath"]}/{this.Name}.png";

        public string LocalSpecialty { get; set; }

        public string Element { get; set; }

        public string Weapon { get; set; }

        public string Enemy { get; set; }

        public string MiniBoss { get; set; }

        public string WeeklyBoss { get; set; }

        public string BookType { get; set; }

        public MaterialRarity Rarity { get; set; }
    }
}