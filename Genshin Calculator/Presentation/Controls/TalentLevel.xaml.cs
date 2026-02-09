using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Genshin_Calculator.Presentation.Controls;

public partial class TalentLevelControl : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(TalentLevelControl), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty LevelProperty =
        DependencyProperty.Register(nameof(Level), typeof(int), typeof(TalentLevelControl), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public TalentLevelControl()
    {
        this.InitializeComponent();
    }

    public string Title
    {
        get => (string)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    public int Level
    {
        get => (int)this.GetValue(LevelProperty);
        set => this.SetValue(LevelProperty, Math.Clamp(value, 1, 10));
    }

    [RelayCommand]
    public void IncreaseLevel()
    {
        if (this.Level < 10)
        {
            this.Level++;
        }
    }

    [RelayCommand]
    public void DecreaseLevel()
    {
        if (this.Level > 1)
        {
            this.Level--;
        }
    }

    private void NumberValidation(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _);
    }
}