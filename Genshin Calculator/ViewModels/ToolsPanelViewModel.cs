using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public partial class ToolsPanelViewModel : ObservableObject
{
    private readonly CharacterService characterService;

    [ObservableProperty]
    private bool isPopupOpen;

    [ObservableProperty]
    private AddCharacterViewModel? selectedCharacter;

    public ToolsPanelViewModel(CharacterService characterService)
    {
        this.characterService = characterService;

        characterService.CharacterAdded += this.OnCharacterAdded;
        characterService.CharacterDeleted += this.OnCharacterDeleted;

        this.AvailableCharacters = this.LoadAvailableCharacters();
    }

    public ObservableCollection<AddCharacterViewModel> AvailableCharacters { get; }

    private ObservableCollection<AddCharacterViewModel> LoadAvailableCharacters()
    {
        var characters = this.characterService
            .GetCharacters()
            .Where(c => c.Deleted)
            .Select(c => new AddCharacterViewModel(
                c.Name,
                ImageService.GetCharacterImage(c.Name)));

        return new ObservableCollection<AddCharacterViewModel>(characters);
    }

    partial void OnSelectedCharacterChanged(AddCharacterViewModel? value)
    {
        ConfirmAddCharacterCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void SelectCharacter(AddCharacterViewModel selected)
    {
        this.SelectedCharacter = selected;
    }

    [RelayCommand(CanExecute = nameof(CanConfirmAddCharacter))]
    private void ConfirmAddCharacter()
    {
        var character = this.characterService.GetCharacterByName(this.SelectedCharacter!.Name);
        this.characterService.AddCharacter(character!);

        this.AvailableCharacters.Remove(this.SelectedCharacter);

        this.IsPopupOpen = false;
    }

    [RelayCommand]
    private void ManageInventory()
    {
        throw new NotSupportedException();
    }

    [RelayCommand]
    private void ManagePriority()
    {
        throw new NotSupportedException();
    }

    private void OnCharacterAdded(Character c)
    {
        var vm = this.AvailableCharacters
            .FirstOrDefault(x => x.Name == c.Name);

        if (vm != null)
        {
            this.AvailableCharacters.Remove(vm);
        }
    }

    private void OnCharacterDeleted(Character c)
    {
        this.AvailableCharacters.Add(
            new AddCharacterViewModel(
                c.Name,
                ImageService.GetCharacterImage(c.Name)));
    }

    private bool CanConfirmAddCharacter() => this.SelectedCharacter != null;

    [RelayCommand]
    private void TogglePopup() => this.IsPopupOpen = !this.IsPopupOpen;
}