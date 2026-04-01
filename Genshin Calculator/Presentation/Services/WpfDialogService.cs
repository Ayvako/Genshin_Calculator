using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using Genshin_Calculator.Presentation.Features.Characters;
using Genshin_Calculator.Presentation.Features.Dialogs;
using Genshin_Calculator.Presentation.Features.Inventory;
using Genshin_Calculator.Services;
using System;
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
        var view = CreateDialog<AddMaterialsDialogView>(vm);

        SetupCloseOnDeactivate(view, onCloseRequested: () => view.Close());
        vm.RequestClose += () => view.Close();

        ShowDialogWithDimming(view);
    }

    public void ShowCharacterEdit(Character character)
    {
        var vm = new CharacterEditViewModel(character, this.characterService);
        var view = CreateDialog<CharacterEditView>(vm);

        vm.RequestClose += () => view.Close();

        ShowDialogWithDimming(view);
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

        ShowDialogWithDimming(view);
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

        ShowDialogWithDimming(view);
    }

    public bool ShowUpgradeCharacterDialog(Character character, List<MaterialRequirement> materialRequirementUIs)
    {
        var vm = new UpgradeCharacterDialogViewModel(character, materialRequirementUIs);
        var view = CreateDialog<UpgradeCharacterDialogView>(vm);

        SetupCloseOnDeactivate(view, onCloseRequested: () => view.Close());
        vm.RequestClose += () => view.Close();

        ShowDialogWithDimming(view);

        return vm.DialogResult ?? false;
    }

    private static TWindow CreateDialog<TWindow>(object viewModel)
        where TWindow : Window, new()
    {
        return new TWindow
        {
            DataContext = viewModel,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
    }

    private static void ShowDialogWithDimming(Window window)
    {
        WeakReferenceMessenger.Default.Send(new DimmingMessage(true));
        try
        {
            window.ShowDialog();
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new DimmingMessage(false));
        }
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
}