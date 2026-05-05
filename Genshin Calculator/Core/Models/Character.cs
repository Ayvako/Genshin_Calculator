using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Genshin_Calculator.Core.Models;

public partial class Character : ObservableObject
{
    [ObservableProperty]
    private Level currentLevel = new(1, false);

    [ObservableProperty]
    private Level desiredLevel = new(1, false);

    [ObservableProperty]
    private Skill autoAttack;

    [ObservableProperty]
    private Skill elemental;

    [ObservableProperty]
    private Skill burst;

    [ObservableProperty]
    private bool deleted = true;

    [ObservableProperty]
    private bool activated;

    [ObservableProperty]
    private int priority;

    public Character(string name, Assets assets)
    {
        this.Name = name;
        this.Assets = assets;
        this.AutoAttack = new Skill() { Name = "AutoAttack" };
        this.Elemental = new Skill() { Name = "Elemental" };
        this.Burst = new Skill() { Name = "Burst" };
    }

    public string Name { get; set; }

    [JsonIgnore]
    public Assets? Assets { get; set; }

    public Character Clone()
    {
        var clone = (Character)this.MemberwiseClone();

        clone.AutoAttack = this.AutoAttack.Clone();
        clone.Elemental = this.Elemental.Clone();
        clone.Burst = this.Burst.Clone();

        return clone;
    }

    public void Reset()
    {
        this.CurrentLevel = new Level(1, false);
        this.DesiredLevel = new Level(1, false);
        this.Activated = false;

        if (this.AutoAttack != null)
        {
            this.AutoAttack.CurrentLevel = 1;
            this.AutoAttack.DesiredLevel = 1;
        }

        if (this.Elemental != null)
        {
            this.Elemental.CurrentLevel = 1;
            this.Elemental.DesiredLevel = 1;
        }

        if (this.Burst != null)
        {
            this.Burst.CurrentLevel = 1;
            this.Burst.DesiredLevel = 1;
        }
    }

    public void ApplyChangesFrom(Character other)
    {
        this.Activated = other.Activated;
        this.Deleted = other.Deleted;
        this.CurrentLevel = other.CurrentLevel;
        this.DesiredLevel = other.DesiredLevel;
        this.Priority = other.Priority;
        this.AutoAttack.CopyLevelsFrom(other.AutoAttack!);
        this.Elemental.CopyLevelsFrom(other.Elemental!);
        this.Burst.CopyLevelsFrom(other.Burst!);
    }

    partial void OnCurrentLevelChanged(Level value)
    {
        if (value.CompareTo(this.DesiredLevel) > 0)
        {
            this.DesiredLevel = value;
        }
    }

    partial void OnDesiredLevelChanged(Level value)
    {
        if (CurrentLevel.CompareTo(value) > 0)
        {
            this.CurrentLevel = value;
        }
    }
}