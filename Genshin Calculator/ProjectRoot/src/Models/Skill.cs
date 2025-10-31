namespace Genshin_Calculator.ProjectRoot.Src.Models
{
    public class Skill
    {
        public int CurrentLevel { get; set; }

        public int DesiredLevel { get; set; }

        public Skill(int currentLevel = 1, int desiredLevel = 1)
        {
            this.CurrentLevel = currentLevel;
            this.DesiredLevel = desiredLevel;
        }
    }
}