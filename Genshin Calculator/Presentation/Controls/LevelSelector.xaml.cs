using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Presentation.Models;

namespace Genshin_Calculator.Presentation.Controls;

public partial class LevelSelectorControl : UserControl
{
    public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(LevelSelectorControl),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty LevelProperty =
    DependencyProperty.Register(
        nameof(Level),
        typeof(string),
        typeof(LevelSelectorControl),
        new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsPopupOpenProperty =
    DependencyProperty.Register(
        nameof(IsPopupOpen),
        typeof(bool),
        typeof(LevelSelectorControl),
        new PropertyMetadata(false));

    public static readonly DependencyProperty LevelOptionsPairsProperty =
    DependencyProperty.Register(
        nameof(LevelOptionsPairs),
        typeof(IReadOnlyList<LevelPair>),
        typeof(LevelSelectorControl),
        new PropertyMetadata(null));

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

    public IReadOnlyList<LevelPair> LevelOptionsPairs
    {
        get => (IReadOnlyList<LevelPair>)this.GetValue(LevelOptionsPairsProperty);
        set => this.SetValue(LevelOptionsPairsProperty, value);
    }

    [RelayCommand]
    public void TogglePopup()
    {
        this.IsPopupOpen = !this.IsPopupOpen;
    }

    [RelayCommand]

    public void SelectLevel(string level)
    {
        this.Level = level ?? string.Empty;
        this.IsPopupOpen = false;
    }

    [RelayCommand]
    public void IncreaseLevel()
    {
        var index = LevelHelper.Levels.IndexOf(this.Level);

        if (index < (LevelHelper.Levels.Length - 1))
        {
            this.Level = LevelHelper.Levels[index + 1];
        }
    }

    [RelayCommand]
    public void DecreaseLevel()
    {
        var index = LevelHelper.Levels.IndexOf(this.Level);

        if (index > 0)
        {
            this.Level = LevelHelper.Levels[index - 1];
        }
    }
}