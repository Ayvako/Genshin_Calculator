using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Genshin_Calculator.Presentation.Features.Dialogs;

public partial class ConfirmDialogViewModel : ObservableObject
{
    [ObservableProperty] private string title;

    [ObservableProperty] private string message;

    public ConfirmDialogViewModel(string title, string message)
    {
        this.Title = title;
        this.Message = message;
    }

    public event Action? RequestClose;

    public bool Result { get; private set; }

    [RelayCommand]
    private void Save()
    {
        this.Result = true;
        this.RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        this.Result = false;
        this.RequestClose?.Invoke();
    }
}