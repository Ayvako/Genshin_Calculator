using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Views;

namespace Genshin_Calculator.ViewModels;

public partial class CharacterCardViewModel : ObservableObject
{
    [ObservableProperty]
    private List<Material> requiredMaterials;

    public CharacterCardViewModel(Character character, List<Material> requiredMaterials)
    {
        this.Character = character;
        this.RequiredMaterials = requiredMaterials;
        this.ImagePath = $"Resources/Images/Characters/{character.Name}.png";

        this.Character.PropertyChanged += (_, _) => this.OnPropertyChanged(string.Empty);
    }

    public CharacterCardViewModel()
    {
        this.Character = new Character("Keqing", new Assets("Keqing", "Sword", "Anemo", "Wolfhook", "TeachingsOfFreedom", "SlimeCondensate", "HurricaneSeed", "DvalinsPlume", 5))
        {
            CurrentLevel = "70",
            DesiredLevel = "90",
            Activated = true,
            Deleted = false,
            AutoAttack = new Skill(1, 10),
            Elemental = new Skill(1, 10),
            Burst = new Skill(1, 10),
        };
        this.RequiredMaterials = [];

        this.RequiredMaterials.Add(new Material("WanderersAdvice", MaterailTypes.Exp, MaterialRarity.Green, 10));
        this.ImagePath = $"Resources/Images/Characters/{this.Character.Name}.png";
    }

    public event Action Edited = null!;

    public Character Character { get; set; }

    public string ImagePath { get; set; }

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
        this.Character.Deleted = true;
        this.Character.Activated = false;
    }
}