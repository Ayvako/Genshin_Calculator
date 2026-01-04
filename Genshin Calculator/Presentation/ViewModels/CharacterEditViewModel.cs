using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class CharacterEditViewModel : ObservableObject
{
    private readonly ImmutableArray<string> levels = LevelHelper.Levels;

    [ObservableProperty]
    private bool isPopupOpen;

    [ObservableProperty]
    private Skill[] talents;

    public CharacterEditViewModel(Character character)
    {
        this.Character = character;
        this.Editable = character.Clone();
        this.talents =
        [
            this.Editable.AutoAttack!,
            this.Editable.Elemental!,
            this.Editable.Burst!,
        ];
    }

    public event Action Saved = null!;

    public event Action RequestClose = null!;

    public List<string[]> LevelOptionsPairs { get; } =

    [
        ["20", "20+"],
        ["40", "40+"],
        ["50", "50+"],
        ["60", "60+"],
        ["70", "70+"],
        ["80", "80+"]
    ];

    public Character Character { get; }

    public Character Editable { get; }

    private static void CopySkillLevels(Skill? target, Skill? source)
    {
        if (target is null || source is null)
        {
            return;
        }

        target.CurrentLevel = source.CurrentLevel;
        target.DesiredLevel = source.DesiredLevel;
    }

    [RelayCommand]
    private static void DragWindow(Window window)
    {
        window?.DragMove();
    }

    [RelayCommand]
    private void TogglePopup() => this.IsPopupOpen = !this.IsPopupOpen;

    [RelayCommand]
    private void SelectLevel(string level)
    {
        this.Editable.CurrentLevel = level;
        this.IsPopupOpen = false;
    }

    [RelayCommand]
    private void IncreaseCurrentCharacterLevel()
    {
        var index = this.levels.IndexOf(this.Editable.CurrentLevel);

        if (index < (this.levels.Length - 1))
        {
            this.Editable.CurrentLevel = this.levels[index + 1];
        }
    }

    [RelayCommand]
    private void DecreaseCurrentCharacterLevel()
    {
        var index = this.levels.IndexOf(this.Editable.CurrentLevel);

        if (index > 0)
        {
            this.Editable.CurrentLevel = this.levels[index - 1];
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (!this.ValidateLevels())
        {
            return;
        }

        this.Character.CurrentLevel = this.Editable.CurrentLevel;
        this.Character.DesiredLevel = this.Editable.DesiredLevel;

        CopySkillLevels(this.Character.AutoAttack, this.Editable.AutoAttack);
        CopySkillLevels(this.Character.Elemental, this.Editable.Elemental);
        CopySkillLevels(this.Character.Burst, this.Editable.Burst);

        this.Saved?.Invoke();
    }

    private bool ValidateLevels()
    {
        var editable = this.Editable;

        var currentIndex = this.levels.IndexOf(editable.CurrentLevel);
        var desiredIndex = this.levels.IndexOf(editable.DesiredLevel);

        if (currentIndex >= 0 && desiredIndex >= 0)
        {
            if (currentIndex > desiredIndex)
            {
                MessageBox.Show("Current Level cannot be greater than Desired Level.");
                return false;
            }
        }
        else
        {
            if (int.TryParse(editable.CurrentLevel, out var current) &&
                int.TryParse(editable.DesiredLevel, out var desired) &&
                current > desired)
            {
                MessageBox.Show("Current Level cannot be greater than Desired Level.");
                return false;
            }
        }

        static bool SkillInvalid(Skill? skill) => skill != null && skill.CurrentLevel > skill.DesiredLevel;

        if (SkillInvalid(editable.AutoAttack))
        {
            MessageBox.Show("Auto Attack Current Level cannot be greater than Desired Level.");
            return false;
        }

        if (SkillInvalid(editable.Elemental))
        {
            MessageBox.Show("Elemental Current Level cannot be greater than Desired Level.");
            return false;
        }

        if (SkillInvalid(editable.Burst))
        {
            MessageBox.Show("Burst Current Level cannot be greater than Desired Level.");
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void Cancel()
    {
        this.RequestClose.Invoke();
    }
}