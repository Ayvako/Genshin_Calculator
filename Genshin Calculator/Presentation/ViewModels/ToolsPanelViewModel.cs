using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Views;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class ToolsPanelViewModel : ObservableRecipient, IRecipient<CharacterChangedMessage>
{
    private readonly CharacterService characterService;

    [ObservableProperty]
    private Character? selectedCharacter;

    [ObservableProperty]
    private ObservableCollection<Character> availableCharacters;

    public ToolsPanelViewModel(CharacterService characterService)
    {
        this.characterService = characterService;
        this.IsActive = true;
        this.AvailableCharacters = this.LoadAvailableCharacters();
    }

    public void Receive(CharacterChangedMessage message)
    {
        var character = message.Value;

        if (character.Deleted)
        {
            this.OnCharacterDeleted(character);
        }
        else
        {
            this.OnCharacterAdded(character);
        }
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