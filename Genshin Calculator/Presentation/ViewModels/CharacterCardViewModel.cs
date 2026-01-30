using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Views;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class CharacterCardViewModel : ObservableRecipient, IRecipient<CharacterChangedMessage>
{
    private readonly CharacterService characterService;

    [ObservableProperty]
    private List<Material> requiredMaterials;

    public CharacterCardViewModel(Character character, List<Material> requiredMaterials, CharacterService characterService)
    {
        this.characterService = characterService;
        this.Character = character;
        this.RequiredMaterials = requiredMaterials;

        this.IsActive = true;
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
        var editVm = new CharacterEditViewModel(this.Character);
        var editWindow = new CharacterEditView
        {
            DataContext = editVm,
        };
        editVm.Saved += () =>
        {
            this.characterService.UpdateCharacter(this.Character);

            WeakReferenceMessenger.Default.Send(new RefreshMaterialsRequestMessage());
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
    private void ToggleActive()
    {
        if (this.Character.Activated)
        {
            this.characterService.DisableCharacter(this.Character);
        }
        else
        {
            this.characterService.EnableCharacter(this.Character);
        }

        this.OnPropertyChanged(nameof(this.IsActivated));
    }

    [RelayCommand]
    private void Remove()
    {
        this.characterService.DeleteCharacter(this.Character);
    }
}