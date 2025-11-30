using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers;

namespace Genshin_Calculator.Controls;

public partial class LevelSelectorControl : UserControl
{

    public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(nameof(Title), typeof(string), typeof(LevelSelectorControl), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty LevelProperty =
    DependencyProperty.Register(nameof(Level), typeof(string), typeof(LevelSelectorControl), new FrameworkPropertyMetadata(
            "1",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsPopupOpenProperty =
    DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(LevelSelectorControl), new PropertyMetadata(false));

    public static readonly DependencyProperty LevelOptionsPairsProperty =
    DependencyProperty.Register(nameof(LevelOptionsPairs), typeof(List<string[]>), typeof(LevelSelectorControl), new PropertyMetadata(null));

    public LevelSelectorControl()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get => (string)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    public string Level
    {
        get => (string)this.GetValue(LevelProperty);
        set => this.SetValue(LevelProperty, value);
    }

    public bool IsPopupOpen
    {
        get => (bool)this.GetValue(IsPopupOpenProperty);
        set => this.SetValue(IsPopupOpenProperty, value);
    }

    public List<string[]> LevelOptionsPairs
    {
        get => (List<string[]>)this.GetValue(LevelOptionsPairsProperty);
        set => this.SetValue(LevelOptionsPairsProperty, value);
    }

    public ICommand TogglePopupCommand => new RelayCommand(() => this.IsPopupOpen = !this.IsPopupOpen);

    public ICommand SelectLevelCommand => new RelayCommand<string>(level =>
    {
        this.Level = level ?? string.Empty;
        this.IsPopupOpen = false;
    });

    public ICommand IncreaseLevelCommand => new RelayCommand(() =>
    {
        var index = LevelHelper.Levels.IndexOf(this.Level);

        if (index < (LevelHelper.Levels.Length - 1))
        {
            this.Level = LevelHelper.Levels[index + 1];
        }
    });

    public ICommand DecreaseLevelCommand => new RelayCommand(() =>
    {
        var index = LevelHelper.Levels.IndexOf(this.Level);

        if (index > 0)
        {
            this.Level = LevelHelper.Levels[index - 1];
        }
    });
}