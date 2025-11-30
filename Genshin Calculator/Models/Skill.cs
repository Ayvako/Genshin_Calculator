using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Helpers;

namespace Genshin_Calculator.Models
{
    public partial class Skill : ObservableObject
    {
        [ObservableProperty]
        private int currentLevel = 1;

        [ObservableProperty]
        private int desiredLevel = 1;

        public Skill(int currentLevel = 1, int desiredLevel = 1)
        {
            this.CurrentLevel = currentLevel;
            this.DesiredLevel = desiredLevel;
        }

        public Skill Clone() => new(this.CurrentLevel, this.DesiredLevel);

        partial void OnCurrentLevelChanged(int value)
        {
            if (value > desiredLevel)
            {
                DesiredLevel = CurrentLevel;
            }
        }

        partial void OnDesiredLevelChanged(int value)
        {
            if (CurrentLevel > value)
            {
                CurrentLevel = value;
            }
        }
    }
}