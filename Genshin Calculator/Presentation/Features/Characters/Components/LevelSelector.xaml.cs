using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Genshin_Calculator.Presentation.Features.Characters.Components;

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
            typeof(Level),
            typeof(LevelSelectorControl),
            new FrameworkPropertyMetadata(
                default(Level),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsPopupOpenProperty =
    DependencyProperty.Register(
        nameof(IsPopupOpen),
        typeof(bool),
        typeof(LevelSelectorControl),
        new PropertyMetadata(false));

    public static readonly DependencyProperty LevelRowsProperty =
    DependencyProperty.Register(
        nameof(LevelRows),
        typeof(IReadOnlyList<LevelOptionRow>),
        typeof(LevelSelectorControl),
        new PropertyMetadata(null));

    private static readonly ImmutableList<Level> Levels = LevelHelper.Levels;

    public LevelSelectorControl()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get => (string)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    public Level Level
    {
        get => (Level)this.GetValue(LevelProperty);
        set => this.SetValue(LevelProperty, value);
    }

    public bool IsPopupOpen
    {
        get => (bool)this.GetValue(IsPopupOpenProperty);
        set => this.SetValue(IsPopupOpenProperty, value);
    }

    public IReadOnlyList<LevelOptionRow> LevelRows
    {
        get => (IReadOnlyList<LevelOptionRow>)this.GetValue(LevelRowsProperty);
        set => this.SetValue(LevelRowsProperty, value);
    }

    [RelayCommand]
    public void SelectLevel(Level level)
    {
        this.Level = level;
        this.IsPopupOpen = false;
    }

    [RelayCommand]
    public void IncreaseLevel()
    {
        var index = Levels.IndexOf(this.Level);

        if (index < Levels.Count - 1)
        {
            this.Level = Levels[index + 1];
        }
    }

    [RelayCommand]
    public void DecreaseLevel()
    {
        var index = Levels.IndexOf(this.Level);

        if (index > 0)
        {
            this.Level = Levels[index - 1];
        }
    }

    private void TogglePopupButtonPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (this.IsPopupOpen)
        {
            this.IsPopupOpen = false;
            e.Handled = true;
        }
    }
}