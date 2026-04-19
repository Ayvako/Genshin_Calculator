using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Features.Characters;
using Genshin_Calculator.Presentation.Features.Dialogs;
using Genshin_Calculator.Presentation.Features.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Genshin_Calculator.Presentation.Services;

public class ViewService : IViewService
{
    private readonly ICharacterService characterService;

    private readonly IInventoryService inventoryService;

    private int openDialogsCount = 0;

    public ViewService(ICharacterService characterService, IInventoryService inventoryService)
    {
        this.characterService = characterService;
        this.inventoryService = inventoryService;
    }

    public void ShowAddMaterialsDialog(List<Material> list)
    {
        var vm = new AddMaterialsDialogViewModel(list);
        var view = CreateDialog<AddMaterialsDialogView>(vm);

        SetupCloseOnDeactivate(view, onCloseRequested: () => view.Close());
        vm.RequestClose += () => view.Close();

        this.ShowDialogWithDimming(view);
    }

    public void ShowCharacterEdit(Character character)
    {
        var vm = new CharacterEditViewModel(character, this.characterService);
        var view = CreateDialog<CharacterEditView>(vm);

        vm.RequestClose += () => view.Close();

        this.ShowDialogWithDimming(view);
    }

    public void ShowCharacterSelector()
    {
        var vm = new CharacterSelectorViewModel(this.characterService);
        var view = CreateDialog<CharacterSelectorView>(vm);

        vm.CloseRequested += (s, result) =>
        {
            view.DialogResult = result;
            view.Close();
        };

        this.ShowDialogWithDimming(view);
    }

    public void ShowInventory()
    {
        var vm = new InventoryViewModel(this.inventoryService);
        var view = CreateDialog<InventoryView>(vm);

        vm.CloseRequested += (s, result) =>
        {
            view.DialogResult = result;
            view.Close();
        };

        this.ShowDialogWithDimming(view);
    }

    public bool ShowUpgradeCharacterDialog(Character character)
    {
        var vm = new UpgradeCharacterDialogViewModel(character, this, this.inventoryService);
        var view = CreateDialog<UpgradeCharacterDialogView>(vm);

        vm.RequestClose += () => view.Close();

        this.ShowDialogWithDimming(view);

        return vm.DialogResult ?? false;
    }

    private static void SetupCloseOnDeactivate(Window window, Action onCloseRequested)
    {
        bool isClosing = false;

        window.Closing += (s, e) => isClosing = true;

        window.Deactivated += (s, e) =>
        {
            if (!isClosing)
            {
                isClosing = true;
                onCloseRequested?.Invoke();
            }
        };
    }

    private static TWindow CreateDialog<TWindow>(object viewModel)
        where TWindow : Window, new()
    {
        var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

        return new TWindow
        {
            DataContext = viewModel,
            Owner = activeWindow ?? Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
    }

    private void ShowDialogWithDimming(Window window)
    {
        this.openDialogsCount++;

        if (this.openDialogsCount == 1)
        {
            WeakReferenceMessenger.Default.Send(new DimmingMessage(true));
        }

        try
        {
            window.ShowDialog();
        }
        finally
        {
            this.openDialogsCount--;

            if (this.openDialogsCount == 0)
            {
                WeakReferenceMessenger.Default.Send(new DimmingMessage(false));
            }
        }
    }
}