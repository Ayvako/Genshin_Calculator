using System;
using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.ViewModels;

public partial class CharacterEditViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isPopupOpen;

    public CharacterEditViewModel(Character character)
    {
        this.Character = character;
        this.Editable = character.Clone();
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
        var index = LevelHelper.Levels.IndexOf(this.Editable.CurrentLevel);

        if (index < (LevelHelper.Levels.Length - 1))
        {
            this.Editable.CurrentLevel = LevelHelper.Levels[index + 1];
        }
    }

    [RelayCommand]
    private void DecreaseCurrentCharacterLevel()
    {
        var index = LevelHelper.Levels.IndexOf(this.Editable.CurrentLevel);

        if (index > 0)
        {
            this.Editable.CurrentLevel = LevelHelper.Levels[index - 1];
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

        this.Character.AutoAttack.CurrentLevel = this.Editable.AutoAttack.CurrentLevel;
        this.Character.AutoAttack.DesiredLevel = this.Editable.AutoAttack.DesiredLevel;

        this.Character.Elemental.CurrentLevel = this.Editable.Elemental.CurrentLevel;
        this.Character.Elemental.DesiredLevel = this.Editable.Elemental.DesiredLevel;

        this.Character.Burst.CurrentLevel = this.Editable.Burst.CurrentLevel;
        this.Character.Burst.DesiredLevel = this.Editable.Burst.DesiredLevel;

        this.Saved?.Invoke();
    }

    private bool ValidateLevels()
    {
        if (int.TryParse(this.Character.CurrentLevel, out var current) &&
            int.TryParse(this.Character.DesiredLevel, out var desired) && current > desired)
        {
            MessageBox.Show("Current Level cannot be greater than Desired Level.");
            return false;
        }

        //добавить проверки для AutoAttack, Elemental, Burst
        return true;
    }

    [RelayCommand]
    private void Cancel()
    {
        this.RequestClose.Invoke();
    }
}