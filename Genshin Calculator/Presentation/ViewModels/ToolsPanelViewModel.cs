using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Views;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class ToolsPanelViewModel : ObservableObject
{
    private readonly CharacterService characterService;

    [ObservableProperty]
    private Character? selectedCharacter;

    [ObservableProperty]
    private ObservableCollection<Character> availableCharacters;

    public ToolsPanelViewModel(CharacterService characterService)
    {
        this.characterService = characterService;

        characterService.CharacterAdded += this.OnCharacterAdded;
        characterService.CharacterDeleted += this.OnCharacterDeleted;

        this.AvailableCharacters = this.LoadAvailableCharacters();
    }

    private ObservableCollection<Character> LoadAvailableCharacters()
    {
        return new ObservableCollection<Character>(
            this.characterService.GetCharacters().Where(c => c.Deleted));
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
        var existing = this.AvailableCharacters.FirstOrDefault(x => x.Name == c.Name);
        if (existing != null)
        {
            this.AvailableCharacters.Remove(existing);
        }
    }

    private void OnCharacterDeleted(Character c)
    {
        if (!this.AvailableCharacters.Any(x => x.Name == c.Name))
        {
            this.AvailableCharacters.Add(c);
        }
    }

    [RelayCommand]
    private void TogglePopup()
    {
        var vm = new CharacterSelectorViewModel(this.AvailableCharacters, this.characterService);

        var window = new CharacterSelectorView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow,
        };

        vm.CloseRequested += (s, result) =>
        {
            window.DialogResult = result;
            window.Close();
        };

        window.ShowDialog();
    }
}