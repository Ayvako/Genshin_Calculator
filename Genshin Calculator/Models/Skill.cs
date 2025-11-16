namespace Genshin_Calculator.Models
{
    public class Skill
    {
        public Skill(int currentLevel = 1, int desiredLevel = 1)
        {
            this.CurrentLevel = currentLevel;
            this.DesiredLevel = desiredLevel;
        }

        public int CurrentLevel { get; set; }

        public int DesiredLevel { get; set; }
    }
}