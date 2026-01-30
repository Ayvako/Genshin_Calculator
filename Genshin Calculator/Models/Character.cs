using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Helpers;
using Newtonsoft.Json;

namespace Genshin_Calculator.Models;

public partial class Character : ObservableObject
{
    [ObservableProperty]
    private string currentLevel = "1";

    [ObservableProperty]
    private string desiredLevel = "1";

    [ObservableProperty]
    private Skill autoAttack;

    [ObservableProperty]
    private Skill elemental;

    [ObservableProperty]
    private Skill burst;

    public Character(string name, Assets assets)
    {
        this.Name = name;
        this.Assets = assets;

        // Later set names based on character data
        this.AutoAttack = new Skill() { Name = "AutoAttack" };
        this.Elemental = new Skill() { Name = "Elemental" };
        this.Burst = new Skill() { Name = "Burst" };
        this.Priority = ++Count;
    }

    public string Name { get; set; }

    public bool Deleted { get; set; } = true;

    public bool Activated { get; set; }

    public int Priority { get; set; }

    [JsonIgnore]
    public Assets? Assets { get; set; }

    private static int Count { get; set; }

    public Character Clone()
    {
        return new Character(this.Name, this.Assets!)
        {
            CurrentLevel = this.CurrentLevel,
            DesiredLevel = this.DesiredLevel,
            AutoAttack = this.AutoAttack.Clone(),
            Elemental = this.Elemental.Clone(),
            Burst = this.Burst.Clone(),
            Activated = this.Activated,
            Deleted = this.Deleted,
            Priority = this.Priority,
        };
    }

    public void ApplyChangesFrom(Character other)
    {
        this.CurrentLevel = other.CurrentLevel;
        this.DesiredLevel = other.DesiredLevel;
        this.AutoAttack.CopyLevelsFrom(other.AutoAttack!);
        this.Elemental.CopyLevelsFrom(other.Elemental!);
        this.Burst.CopyLevelsFrom(other.Burst!);
    }

    partial void OnCurrentLevelChanged(string value)
    {
        if (LevelHelper.CompareLevels(value, DesiredLevel) > 0)
        {
            DesiredLevel = CurrentLevel;
        }
    }

    partial void OnDesiredLevelChanged(string value)
    {
        if (LevelHelper.CompareLevels(CurrentLevel, value) > 0)
        {
            CurrentLevel = value;
        }
    }
}