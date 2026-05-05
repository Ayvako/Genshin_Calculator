using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterViewModel : ObservableObject
{
    [ObservableProperty]
    private Level currentLevel;

    [ObservableProperty]
    private Level desiredLevel;

    [ObservableProperty]
    private Skill autoAttack;

    [ObservableProperty]
    private Skill elemental;

    [ObservableProperty]
    private Skill burst;

    [ObservableProperty]
    private bool deleted;

    [ObservableProperty]
    private bool activated;

    [ObservableProperty]
    private int priority;

    public CharacterViewModel(Character model)
    {
        this.Model = model;
        this.currentLevel = model.CurrentLevel;
        this.desiredLevel = model.DesiredLevel;
        this.autoAttack = model.AutoAttack;
        this.elemental = model.Elemental;
        this.burst = model.Burst;
        this.deleted = model.Deleted;
        this.activated = model.Activated;
        this.priority = model.Priority;
    }

    public Character Model { get; }

    public string Name => this.Model.Name;

    public Assets? Assets => this.Model.Assets;

    public CharacterViewModel Clone()
    {
        return new CharacterViewModel(this.Model.Clone());
    }

    public void ApplyChangesFrom(CharacterViewModel other)
    {
        this.Model.ApplyChangesFrom(other.Model);

        this.CurrentLevel = this.Model.CurrentLevel;
        this.DesiredLevel = this.Model.DesiredLevel;
        this.Activated = this.Model.Activated;
        this.Deleted = this.Model.Deleted;
        this.Priority = this.Model.Priority;
    }

    public void SyncToModel()
    {
        this.Model.CurrentLevel = this.CurrentLevel;
        this.Model.DesiredLevel = this.DesiredLevel;
        this.Model.Activated = this.Activated;
        this.Model.Deleted = this.Deleted;
        this.Model.Priority = this.Priority;
        this.Model.AutoAttack.CopyLevelsFrom(this.AutoAttack);
        this.Model.Elemental.CopyLevelsFrom(this.Elemental);
        this.Model.Burst.CopyLevelsFrom(this.Burst);
    }

    partial void OnCurrentLevelChanged(Level value)
    {
        if (value.CompareTo(this.DesiredLevel) > 0)
            this.DesiredLevel = value;
    }

    partial void OnDesiredLevelChanged(Level value)
    {
        if (this.CurrentLevel.CompareTo(value) > 0)
            this.CurrentLevel = value;
    }
}