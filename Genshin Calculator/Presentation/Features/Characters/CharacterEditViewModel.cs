using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterEditViewModel : ObservableObject, IDisposable
{
    private readonly ICharacterService characterService;

    private readonly ITalentLevelRules rules;

    [ObservableProperty]
    private bool isPopupOpen;

    [ObservableProperty]
    private int maxTalentLevel;

    private bool isUpdating;

    private bool disposed;

    public CharacterEditViewModel(
        Character character,
        ICharacterService characterService,
        ITalentLevelRules rules)
    {
        this.characterService = characterService;
        this.rules = rules;
        this.Character = character;
        this.Editable = character.Clone();

        this.Talents = [this.Editable.AutoAttack!, this.Editable.Elemental!, this.Editable.Burst!];
        this.InitializeLogic();
    }

    public event Action? RequestClose;

    public static IReadOnlyList<LevelOptionRow> LevelRows { get; } =
    [
        new(new Level(1,  false)),
        new(new Level(20, false), new Level(20, true)),
        new(new Level(40, false), new Level(40, true)),
        new(new Level(50, false), new Level(50, true)),
        new(new Level(60, false), new Level(60, true)),
        new(new Level(70, false), new Level(70, true)),
        new(new Level(80, false), new Level(80, true)),
        new(new Level(90, false)),
        new(new Level(95, false), new Level(100, false)),
    ];

    public IReadOnlyList<Skill> Talents { get; }

    public Character Character { get; }

    public Character Editable { get; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.UnsubscribeEvents();
            }

            this.disposed = true;
        }
    }

    [RelayCommand]
    private void TogglePopup() => this.IsPopupOpen = !this.IsPopupOpen;

    [RelayCommand]
    private void SelectLevel(Level level)
    {
        this.Editable.CurrentLevel = level;
        this.IsPopupOpen = false;
    }

    [RelayCommand]
    private void IncreaseCurrentCharacterLevel() => this.ChangeLevel(+1);

    [RelayCommand]
    private void DecreaseCurrentCharacterLevel() => this.ChangeLevel(-1);

    [RelayCommand]
    private void Cancel() => this.RequestClose?.Invoke();

    [RelayCommand]
    private async Task SaveAsync()
    {
        this.Character.ApplyChangesFrom(this.Editable);
        await this.characterService.UpdateCharacterAsync(this.Character);
        this.RequestClose?.Invoke();
    }

    private void ChangeLevel(int delta)
    {
        var allLevels = LevelHelper.Levels;

        int index = allLevels.IndexOf(this.Editable.CurrentLevel);
        if (index == -1)
        {
            return;
        }

        int newIndex = Math.Clamp(index + delta, 0, allLevels.Count - 1);
        this.Editable.CurrentLevel = allLevels[newIndex];
    }

    private void InitializeLogic()
    {
        foreach (var talent in this.Talents)
        {
            talent.PropertyChanged += this.OnTalentPropertyChanged;
        }

        this.Editable.PropertyChanged += this.OnHeroPropertyChanged;

        this.ClampTalents();
    }

    private void OnTalentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (this.isUpdating || sender is not Skill talent)
        {
            return;
        }

        if (e.PropertyName is not (nameof(Skill.CurrentLevel) or nameof(Skill.DesiredLevel)))
        {
            return;
        }

        this.WithUpdate(() => this.ApplyTalentConstraints(talent, e.PropertyName));
    }

    private void ApplyTalentConstraints(Skill talent, string? propertyName)
    {
        bool isCurrent = propertyName == nameof(Skill.CurrentLevel);
        int targetLevel = isCurrent ? talent.CurrentLevel : talent.DesiredLevel;

        if (isCurrent && talent.CurrentLevel > talent.DesiredLevel)
        {
            talent.DesiredLevel = talent.CurrentLevel;
        }
        else if (!isCurrent && talent.DesiredLevel < talent.CurrentLevel)
        {
            talent.CurrentLevel = talent.DesiredLevel;
        }

        var required = this.rules.GetRequiredLevel(targetLevel);

        if (this.Editable.DesiredLevel.CompareTo(required) < 0)
        {
            this.Editable.DesiredLevel = required;
        }

        if (isCurrent && this.Editable.CurrentLevel.CompareTo(required) < 0)
        {
            this.Editable.CurrentLevel = required;
        }

        this.MaxTalentLevel = this.rules.GetMaxTalentLevel(this.Editable.CurrentLevel);
    }

    private void OnHeroPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (this.isUpdating)
        {
            return;
        }

        if (e.PropertyName is nameof(this.Character.CurrentLevel) or nameof(this.Character.DesiredLevel))
        {
            this.WithUpdate(this.ClampTalents);
        }
    }

    private void WithUpdate(Action action)
    {
        if (this.isUpdating)
        {
            return;
        }

        this.isUpdating = true;
        try
        {
            action();
        }
        finally
        {
            this.isUpdating = false;
        }
    }

    private void ClampTalents()
    {
        int currentMax = this.rules.GetMaxTalentLevel(this.Editable.CurrentLevel);

        this.MaxTalentLevel = currentMax;

        foreach (var talent in this.Talents)
        {
            talent.DesiredLevel = this.rules.ClampTalentLevel(this.Editable.DesiredLevel, talent.DesiredLevel);
            talent.CurrentLevel = this.rules.ClampTalentLevel(this.Editable.CurrentLevel, talent.CurrentLevel);
        }
    }

    private void UnsubscribeEvents()
    {
        foreach (var talent in this.Talents)
        {
            talent.PropertyChanged -= this.OnTalentPropertyChanged;
        }

        this.Editable.PropertyChanged -= this.OnHeroPropertyChanged;
    }
}