using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Presentation.Features.Inventory;
using Genshin_Calculator.Presentation.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterCardViewModel : ObservableRecipient, IRecipient<CharacterChangedMessage>
{
    private readonly IViewService dialogService;

    private readonly IInventoryService inventoryService;

    private readonly ICharacterService characterService;

    [ObservableProperty]
    private List<MaterialRequirementViewModel> requiredMaterials;

    public CharacterCardViewModel(CharacterViewModel character, List<MaterialRequirementViewModel> requiredMaterials, IViewService dialogService, IInventoryService inventoryService, ICharacterService characterService)
    {
        this.Character = character;
        this.RequiredMaterials = requiredMaterials;
        this.inventoryService = inventoryService;

        this.IsActive = true;
        this.dialogService = dialogService;
        this.characterService = characterService;
    }

    public CharacterViewModel Character { get; }

    public Assets Assets => this.Character.Assets!;

    public string Name => this.Character.Name;

    public bool IsActivated => this.Character.Activated;

    public string CurrentLevel => this.Character.CurrentLevel.ToString();

    public string DesiredLevel => this.Character.DesiredLevel.ToString();

    public Skill AutoAttack => this.Character.AutoAttack;

    public Skill Elemental => this.Character.Elemental;

    public Skill Burst => this.Character.Burst;

    public void Receive(CharacterChangedMessage message)
    {
        if (message.Value == this.Character.Model)
        {
            this.OnPropertyChanged(string.Empty);
        }
    }

    [RelayCommand]
    private void Edit()
    {
        this.dialogService.ShowCharacterEdit(this.Character);
    }

    [RelayCommand]
    private void OpenAddItem(MaterialViewModel material)
    {
        var relatedMaterials = this.inventoryService
            .GetRelatedMaterials(this.Character.Model, material.Model)
            .Select(m => new MaterialViewModel(m))
            .ToList();

        this.dialogService.ShowAddMaterialsDialog(relatedMaterials);
    }

    [RelayCommand]
    private async Task AscendAsync()
    {
        bool? isConfirmed = this.dialogService.ShowUpgradeCharacterDialog(this.Character);

        if (isConfirmed == true)
        {
            this.inventoryService.Upgrade(this.Character.Model);

            await this.characterService.UpdateCharacterAsync(this.Character.Model);

            Debug.WriteLine("Upgrade completed and saved.");
        }
    }

    [RelayCommand]
    private async Task ToggleActiveAsync()
    {
        await this.characterService.ToggleCharacterActivityAsync(this.Character.Model);
        this.Character.Activated = this.Character.Model.Activated;
        this.OnPropertyChanged(nameof(this.IsActivated));
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        await this.characterService.DeleteCharacterAsync(this.Character.Model);
    }
}