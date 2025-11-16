using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.ViewModels;

public partial class CharacterCardViewModel : ObservableObject
{
    public CharacterCardViewModel(Character character)
    {
        this.Character = character;

        this.ImagePath = $"Resources/Images/Characters/{character.Name}.png";
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

        this.ImagePath = $"Resources/Images/Characters/{this.Character.Name}.png";
    }

    public Character Character { get; set; }

    public string ImagePath { get; set; }

    //public IEnumerable<MissingMaterialViewModel> MissingMaterials
    //=> this.inventoryService.CalculateMissingMaterials(this.Inventory) is var dict && dict != null && dict.ContainsKey(this.Character)
    //    ? dict[this.Character].Select(m => new MissingMaterialViewModel(m))
    //   : [];

    public string Name => this.Character.Name;

    public string CurrentLevel => this.Character.CurrentLevel;

    public string DesiredLevel => this.Character.DesiredLevel;

    public Skill AutoAttack => this.Character.AutoAttack;

    public Skill Elemental => this.Character.Elemental;

    public Skill Burst => this.Character.Burst;

    [RelayCommand]
    private void ToggleActive() => this.Character.Activated = !this.Character.Activated;

    [RelayCommand]
    private void Remove()
    {
        this.Character.Deleted = true;
        this.Character.Activated = false;
    }
}