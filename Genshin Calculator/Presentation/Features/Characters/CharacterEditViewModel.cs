using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterEditViewModel : ObservableObject
{
    private readonly ImmutableArray<string> levels = LevelHelper.Levels;

    private readonly CharacterService characterService;

    [ObservableProperty]
    private bool isPopupOpen;

    [ObservableProperty]
    private int maxTalentLevel;

    public CharacterEditViewModel(Character character, CharacterService characterService)
    {
        this.characterService = characterService;
        this.Character = character;
        this.Editable = character.Clone();

        this.Talents = [this.Editable.AutoAttack!, this.Editable.Elemental!, this.Editable.Burst!];
        this.InitializeLogic();
    }

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

    private static string EnsureLevelIsSufficient(string heroLevel, int talentLevel)
    {
        return talentLevel > LevelHelper.GetMaxTalentLevel(heroLevel)
            ? LevelHelper.GetRequiredLevelForTalent(talentLevel)
            : heroLevel;
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
        this.Character.ApplyChangesFrom(this.Editable);
        this.characterService.UpdateCharacter(this.Character);
        this.RequestClose?.Invoke();
    }

    private void ChangeLevel(int delta)
    {
        int index = this.levels.IndexOf(this.Editable.CurrentLevel);
        int newIndex = Math.Clamp(index + delta, 0, this.levels.Length - 1);
        if (index != -1)
        {
            this.Editable.CurrentLevel = this.levels[newIndex];
        }
    }

    private void InitializeLogic()
    {
        foreach (var talent in this.Talents)
        {
            talent.PropertyChanged += this.OnTalentPropertyChanged;
        }

        this.Editable.PropertyChanged += this.OnHeroPropertyChanged;

        this.UpdateMaxTalentLevel();
    }

    private void OnTalentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not Skill talent)
        {
            return;
        }

        if (e.PropertyName == nameof(Skill.CurrentLevel))
        {
            this.Editable.CurrentLevel = EnsureLevelIsSufficient(this.Editable.CurrentLevel, talent.CurrentLevel);
        }
        else if (e.PropertyName == nameof(Skill.DesiredLevel))
        {
            this.Editable.DesiredLevel = EnsureLevelIsSufficient(this.Editable.DesiredLevel, talent.DesiredLevel);
        }
    }

    private void OnHeroPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(this.Character.CurrentLevel))
        {
            this.UpdateMaxTalentLevel();
            this.SyncTalents(isDesired: false);
        }
        else if (e.PropertyName == nameof(this.Character.DesiredLevel))
        {
            this.SyncTalents(isDesired: true);
        }
    }

    private void UpdateMaxTalentLevel()
        => this.MaxTalentLevel = LevelHelper.GetMaxTalentLevel(this.Editable.CurrentLevel);

    private void SyncTalents(bool isDesired)
    {
        var level = isDesired ? this.Editable.DesiredLevel : this.Editable.CurrentLevel;
        int limit = LevelHelper.GetMaxTalentLevel(level);

        foreach (var talent in this.Talents)
        {
            if (isDesired)
            {
                talent.DesiredLevel = Math.Min(talent.DesiredLevel, limit);
            }
            else
            {
                talent.CurrentLevel = Math.Min(talent.CurrentLevel, limit);
            }
        }
    }
}