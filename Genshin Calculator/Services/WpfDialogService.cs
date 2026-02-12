using System.Windows;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.ViewModels;
using Genshin_Calculator.Presentation.Views;
using Genshin_Calculator.Services.Interfaces;

namespace Genshin_Calculator.Services;

public class WpfDialogService : IDialogService
{
    private readonly CharacterService characterService;

    private readonly InventoryService inventoryService;

    public WpfDialogService(CharacterService characterService, InventoryService inventoryService)
    {
        this.characterService = characterService;
        this.inventoryService = inventoryService;
    }

    public void ShowCharacterEdit(Character character)
    {
        var vm = new CharacterEditViewModel(character, this.characterService);
        var view = new CharacterEditView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        vm.RequestClose += () => view.Close();
        view.ShowDialog();
    }

    public void ShowCharacterSelector()
    {
        var vm = new CharacterSelectorViewModel(this.characterService);

        var window = new CharacterSelectorView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        vm.CloseRequested += (s, result) =>
        {
            window.DialogResult = result;
            window.Close();
        };

        window.ShowDialog();
    }

    public void ShowInventory()
    {
        var vm = new InventoryViewModel(this.inventoryService);

        var window = new InventoryView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        vm.CloseRequested += (s, result) =>
        {
            window.DialogResult = result;
            window.Close();
        };

        window.ShowDialog();
    }

    public void ShowPriority()
    {
        MessageBox.Show("Priority Manager - Coming Soon");
    }
}