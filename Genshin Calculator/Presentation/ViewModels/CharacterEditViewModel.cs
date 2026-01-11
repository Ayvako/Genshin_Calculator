using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Models;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class CharacterEditViewModel : ObservableObject
{
    private readonly ImmutableArray<string> levels = LevelHelper.Levels;

    [ObservableProperty]
    private bool isPopupOpen;

    public CharacterEditViewModel(Character character)
    {
        this.Character = character;
        this.Editable = character.Clone();
        this.Talents =
        [
            this.Editable.AutoAttack!,
            this.Editable.Elemental!,
            this.Editable.Burst!
        ];
    }

    public event Action? Saved;

    public event Action? RequestClose;

    public IReadOnlyList<Skill> Talents { get; }

    public IReadOnlyList<LevelPair> LevelOptionsPairs { get; } =
    [
        new LevelPair("20", "20★"),
        new LevelPair("40", "40★"),
        new LevelPair("50", "50★"),
        new LevelPair("60", "60★"),
        new LevelPair("70", "70★"),
        new LevelPair("80", "80★"),
    ];

    public Character Character { get; }

    public Character Editable { get; }

    [RelayCommand]
    private void TogglePopup() => this.IsPopupOpen = !this.IsPopupOpen;

    [RelayCommand]
    private void SelectLevel(string level)
    {
        this.Editable.CurrentLevel = level;
        this.IsPopupOpen = false;
    }

    [RelayCommand]
    private void IncreaseCurrentCharacterLevel() => this.ChangeLevel(1);

    [RelayCommand]
    private void DecreaseCurrentCharacterLevel() => this.ChangeLevel(-1);

    [RelayCommand]
    private void Cancel()
    {
        this.RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        if (!this.ValidateLevels())
        {
            return;
        }

        this.Character.ApplyChangesFrom(this.Editable);

        this.Saved?.Invoke();
    }

    private void ChangeLevel(int delta)
    {
        var index = this.GetLevelIndex(this.Editable.CurrentLevel);

        if (index < 0)
        {
            return;
        }

        var newIndex = index + delta;

        if (newIndex >= 0 && newIndex < this.levels.Length)
        {
            this.Editable.CurrentLevel = this.levels[newIndex];
        }
    }

    private int GetLevelIndex(string level) => this.levels.IndexOf(level);

    private bool ValidateLevels()
    {
        var currentIndex = this.GetLevelIndex(this.Editable.CurrentLevel);
        var desiredIndex = this.GetLevelIndex(this.Editable.DesiredLevel);

        if (currentIndex > desiredIndex)
        {
            return false;
        }

        foreach (var skill in this.Talents)
        {
            if (skill.CurrentLevel > skill.DesiredLevel)
            {
                return false;
            }
        }

        return true;
    }
}