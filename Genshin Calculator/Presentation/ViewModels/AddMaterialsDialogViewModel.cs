using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Messages;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class AddMaterialsDialogViewModel : ObservableObject
{
    public AddMaterialsDialogViewModel(List<Material> family)
    {
        this.MaterialWrappers = new ObservableCollection<MaterialAdditionWrapper>(
                    family.Select(m => new MaterialAdditionWrapper(m)));
    }

    public event Action? RequestClose;

    public ObservableCollection<MaterialAdditionWrapper> MaterialWrappers { get; }

    [RelayCommand]
    private void Save()
    {
        foreach (var wrapper in this.MaterialWrappers)
        {
            wrapper.Material.Amount += wrapper.AdditionAmount;
        }

        WeakReferenceMessenger.Default.Send(new InventoryChangedMessage());
        this.RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Close()
    {
        this.RequestClose?.Invoke();
    }
}