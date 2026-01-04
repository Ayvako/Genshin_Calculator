using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Views;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class CharacterCardViewModel : ObservableObject
{
    private readonly CharacterService characterService;

    [ObservableProperty]
    private List<Material> requiredMaterials;

    public CharacterCardViewModel(Character character, List<Material> requiredMaterials, CharacterService characterService)
    {
        this.characterService = characterService;
        this.Character = character;
        this.RequiredMaterials = requiredMaterials;
        this.Character.PropertyChanged += (_, _) => this.OnPropertyChanged(string.Empty);
    }

    public event Action Edited = null!;

    public Character Character { get; set; }

    public Assets Assets => this.Character.Assets!;

    public string Name => this.Character.Name;

    public string CurrentLevel => this.Character.CurrentLevel;

    public string DesiredLevel => this.Character.DesiredLevel;

    public Skill AutoAttack => this.Character.AutoAttack;

    public Skill Elemental => this.Character.Elemental;

    public Skill Burst => this.Character.Burst;

    [RelayCommand]
    private void Edit()
    {
        var editVm = new CharacterEditViewModel(this.Character);
        var editWindow = new CharacterEditView
        {
            DataContext = editVm,
        };
        editVm.Saved += () =>
        {
            this.Edited?.Invoke();
            editWindow.Close();
        };

        editVm.RequestClose += () =>
        {
            editWindow.Close();
        };

        editWindow.ShowDialog();
    }

    [RelayCommand]
    private void Ascend() => this.Character.Activated = !this.Character.Activated;

    [RelayCommand]
    private void ToggleActive() => this.Character.Activated = !this.Character.Activated;

    [RelayCommand]
    private void Remove()
    {
        this.characterService.DeleteCharacter(this.Character);
    }
}