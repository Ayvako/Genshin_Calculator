using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Features.Characters;
using Genshin_Calculator.Presentation.Features.Dialogs;
using Genshin_Calculator.Presentation.Features.Inventory;
using Genshin_Calculator.Services;
using System.Collections.Generic;
using System.Windows;

namespace Genshin_Calculator.Presentation.Services;

public class WpfDialogService : IDialogService
{
    private readonly ICharacterService characterService;

    private readonly IInventoryService inventoryService;

    public WpfDialogService(ICharacterService characterService, IInventoryService inventoryService)
    {
        this.characterService = characterService;
        this.inventoryService = inventoryService;
    }

    public void ShowAddMaterialsDialog(List<Material> list)
    {
        var vm = new AddMaterialsDialogViewModel(list);
        var view = new AddMaterialsDialogView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        bool isClosing = false;
        WeakReferenceMessenger.Default.Send(new DimmingMessage(true));

        void SafeClose()
        {
            if (!isClosing)
            {
                isClosing = true;
                WeakReferenceMessenger.Default.Send(new DimmingMessage(false));
                view.Close();
            }
        }

        view.Deactivated += (s, e) => SafeClose();

        vm.RequestClose += () => SafeClose();

        view.ShowDialog();
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
        WeakReferenceMessenger.Default.Send(new DimmingMessage(true));

        view.Closed += (s, e) =>
        {
            WeakReferenceMessenger.Default.Send(new DimmingMessage(false));
        };

        vm.RequestClose += () =>
        {
            view.Close();
        };

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

        WeakReferenceMessenger.Default.Send(new DimmingMessage(true));

        window.Closed += (s, e) =>
        {
            WeakReferenceMessenger.Default.Send(new DimmingMessage(false));
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

        WeakReferenceMessenger.Default.Send(new DimmingMessage(true));

        window.Closed += (s, e) =>
        {
            WeakReferenceMessenger.Default.Send(new DimmingMessage(false));
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