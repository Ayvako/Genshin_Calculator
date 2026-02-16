using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;
using Genshin_Calculator.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class CharacterCardViewModel : ObservableRecipient, IRecipient<CharacterChangedMessage>
{
    private readonly CharacterService characterService;

    private readonly IDialogService dialogService;

    private readonly InventoryService inventoryService;

    [ObservableProperty]
    private List<Material> requiredMaterials;

    public CharacterCardViewModel(Character character, List<Material> requiredMaterials, CharacterService characterService, IDialogService dialogService, InventoryService inventoryService)
    {
        this.characterService = characterService;
        this.Character = character;
        this.RequiredMaterials = requiredMaterials;
        this.inventoryService = inventoryService;

        this.IsActive = true;
        this.dialogService = dialogService;
    }

    public Character Character { get; }

    public Assets Assets => this.Character.Assets!;

    public string Name => this.Character.Name;

    public bool IsActivated => this.Character.Activated;

    public string CurrentLevel => this.Character.CurrentLevel;

    public string DesiredLevel => this.Character.DesiredLevel;

    public Skill AutoAttack => this.Character.AutoAttack;

    public Skill Elemental => this.Character.Elemental;

    public Skill Burst => this.Character.Burst;

    public void Receive(CharacterChangedMessage message)
    {
        if (message.Value == this.Character)
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
    private void OpenAddItem(Material material)
    {
        var relatedMaterials = this.inventoryService.GetRelatedMaterials(this.Character, material);
        this.dialogService.ShowAddMaterialsDialog(relatedMaterials);
    }

    [RelayCommand]
    private void Ascend() => throw new NotImplementedException();

    [RelayCommand]
    private void ToggleActive()
    {
        this.characterService.ToggleCharacterActivity(this.Character);
    }

    [RelayCommand]
    private void Remove()
    {
        this.characterService.DeleteCharacter(this.Character);
    }
}