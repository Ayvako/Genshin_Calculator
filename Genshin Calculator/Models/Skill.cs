using CommunityToolkit.Mvvm.ComponentModel;

namespace Genshin_Calculator.Models;

public partial class Skill : ObservableObject
{
    public const int MinLevel = 1;

    public const int MaxLevel = 10;

    [ObservableProperty]
    private int currentLevel = MinLevel;

    [ObservableProperty]
    private int desiredLevel = MinLevel;

    public Skill(int currentLevel = MinLevel, int desiredLevel = MinLevel)
    {
        this.CurrentLevel = currentLevel;
        this.DesiredLevel = desiredLevel;
    }

    public string Name { get; set; } = string.Empty;

    public Skill Clone() => new(this.CurrentLevel, this.DesiredLevel) { Name = this.Name };

    public void CopyLevelsFrom(Skill source)
    {
        this.CurrentLevel = source.CurrentLevel;
        this.DesiredLevel = source.DesiredLevel;
    }

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