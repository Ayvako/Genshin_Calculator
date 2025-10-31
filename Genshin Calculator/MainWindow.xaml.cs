using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Genshin_Calculator.ProjectRoot.Src.Models;
using Genshin_Calculator.ProjectRoot.Src.Services;
using Genshin_Calculator.ProjectRoot.Src.Upgrades;
using Genshin_Calculator.ProjectRoot.Src.ViewModels;

namespace Genshin_Calculator;

public partial class MainWindow : Window
{
    private static List<Character> AllCharactersList;
    private static Dictionary<string, ImageSource> imageDictionaryCharacter;
    private static Dictionary<string, ImageSource> imageDictionaryMaterial;
    private static Dictionary<string, ImageSource> imageDictionaryTools;
    private static Inventory inventory;

    InventoryService inventoryService;

    public MainWindow()
    {
        CharacterUpgradeService characterService = new CharacterUpgradeService();
        SkillUpgradeService skillUpgradeService = new SkillUpgradeService();
        inventoryService = new InventoryService(characterService, skillUpgradeService);

        this.InitializeComponent();
        this.DataContext = new MainViewModel();


        inventory = DataIO.Import(out AllCharactersList);

        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.MinHeight = 500;
        this.MinWidth = 500;
        this.Background = new LinearGradientBrush()
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1),
            GradientStops =
            {
                new GradientStop((Color)ColorConverter.ConvertFromString("#31394E"), 0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#1D212D"), 0.2),
                new GradientStop((Color)ColorConverter.ConvertFromString("#191E33"), 0.4),
                new GradientStop((Color)ColorConverter.ConvertFromString("#141829"), 0.6),
                new GradientStop((Color)ColorConverter.ConvertFromString("#101321"), 1),
            },
        };
        imageDictionaryCharacter = ImageService.LoadCharacterImages(AllCharactersList);
        imageDictionaryMaterial = ImageService.LoadMaterialImages(inventory.Materials);
        imageDictionaryTools = ImageService.LoadToolsImages();
        this.Content = MainPanel();
    }

    private ScrollViewer MainPanel()
    {
        var ActiveCharactersList = inventory.ActiveCharacters.ToList();

        StackPanel panel = new()
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                CreateToolsPanel(),
                CreateFarmPanel(ActiveCharactersList),
            },
        };

        ScrollViewer scrollViewer = new()
        {
            Content = panel,

            Resources = new ResourceDictionary()
            {
                { typeof(ScrollBar), ScrollStyle() },
            },

            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
        };
        return scrollViewer;
    }

    private StackPanel CreateFarmPanel(List<Character> characters)
    {
        WrapPanel wrapPanel = new();

        foreach (var c in characters)
        {
            StackPanel characterPanel = new()
            {
                Orientation = Orientation.Vertical,
                Children =
                {
                    CreateTableContentsPanel(c),
                    CreateInfoPanel(c),
                    CreateResourcesPanel(c),
                },
            };

            Border characterBorder = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Child = characterPanel,
                MaxWidth = 712,
                MinWidth = 350,
                CornerRadius = new CornerRadius(10),

                Background = new LinearGradientBrush()
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops =
                    {
                        new GradientStop(Color.FromRgb(49, 57, 78), 0),
                        new GradientStop(Color.FromRgb(29, 33, 45), 0.2),
                        new GradientStop(Color.FromRgb(25, 30, 51), 0.4),
                        new GradientStop(Color.FromRgb(20, 24, 41), 0.6),
                        new GradientStop(Color.FromRgb(16, 19, 33), 0.8),
                    },
                },
                Effect = new DropShadowEffect()
                {
                    Color = Colors.Black,
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = 11,
                    Opacity = 0.75,
                },
            };
            wrapPanel.Children.Add(characterBorder);
        }

        return new StackPanel()
        {
            Orientation = Orientation.Vertical,

            Margin = new Thickness(5),
            Children = { wrapPanel },
        };
    }

    private WrapPanel CreateResourcesPanel(Character c)
    {
        var materials = inventoryService.CalculateMissingMaterials(inventory);

        WrapPanel resourcesPanel = new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5),
        };
        if(materials!=null)
        {
            foreach (var m in materials[c])
        {
            StackPanel amountPanel = new()
            {
                Orientation = Orientation.Vertical,
            };

            TextBlock amountText = new()
            {
                Foreground = Brushes.White,
                Text = m.Amount.ToString(),
                TextAlignment = TextAlignment.Center,
            };

            Border amountBorder = new()
            {
                Child = amountText,
                Background = Brushes.Black,
                CornerRadius = new CornerRadius(10, 10, 0, 0),
            };

            Image materialImage = new()
            {
                Source = imageDictionaryMaterial[m.Name],
                Stretch = Stretch.None,
            };
            amountPanel.Children.Add(amountBorder);
            amountPanel.Children.Add(materialImage);

            Border materialBorder = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = amountPanel,
                CornerRadius = new CornerRadius(10),

                Background = SetBackgroundRarity(m.Rarity),
            };

            resourcesPanel.Children.Add(materialBorder);
        }
        }

        return resourcesPanel;
    }

    private static Grid CreateInfoPanel(Character c)
    {
        Image avatar = new()
        {
            Width = 128,
            Height = 128,
            Source = imageDictionaryCharacter[c.Name],
        };
        WrapPanel avatarPanel = new()
        {
            Children = { avatar },
            Background = SetBackgroundRarity(c.Assets.Rarity),
        };
        Border avatarBorder = new()
        {
            Width = 128,
            Height = 128,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(2),
            Child = avatarPanel,
        };
        TextBlock levelTextBlock = new()
        {
            Text = "Levels",
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
        };
        TextBlock levelUpTextBlock = new()
        {
            Text = $"{c.CurrentLevel} -> {c.DesiredLevel}",
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC")),
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            FontSize = 14,
        };
        StackPanel levelPanel = new()
        {
            Orientation = Orientation.Vertical,
            Children =
                {
                    levelTextBlock,
                    levelUpTextBlock,
                },
        };
        TextBlock talentTextBlock = new()
        {
            Text = "Talents",
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
        };
        TextBlock talentsLevelUpTextBlock = new()
        {
            Text =
            $"{c.AutoAttack.CurrentLevel} -> {c.AutoAttack.DesiredLevel}\n" +
            $"{c.Elemental.CurrentLevel} -> {c.Elemental.DesiredLevel}\n" +
            $"{c.Burst.CurrentLevel} -> {c.Burst.DesiredLevel}",
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC")),
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            FontSize = 14,
        };
        StackPanel talentsPanel = new()
        {
            Orientation = Orientation.Vertical,
            Children =
                {
                    talentTextBlock,
                    talentsLevelUpTextBlock,
                },
        };
        StackPanel statsPanel = new()
        {
            Orientation = Orientation.Vertical,
            Children =
                {
                    levelPanel,
                    talentsPanel,
                },
        };
        Grid.SetColumnSpan(avatarBorder, 2);
        Grid.SetColumn(statsPanel, 2);

        Grid infoGrid = new()
        {
            ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star),
                    },
                },
            Children =
                {
                    avatarBorder,
                    statsPanel,
                },
        };

        return infoGrid;
    }

    private static Grid CreateTableContentsPanel(Character c)
    {
        Grid grid = new() { Background = SetBackgroundRarity(c.Assets.Rarity) };

        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

        Image editButton = CreateImage("edit_icon_default.png", "edit_icon_mouseover.png", 30, 30);
        Image ascendButton = CreateImage("acsend_icon_default.png", "acsend_icon_mouseover.png", 30, 30);
        Image activeButton = CreateImage("active_icon_default.png", "active_icon_mouseover.png", 30, 30);
        Image removeButton = CreateImage("remove_icon_default.png", "remove_icon_mouseover.png", 30, 30);

        removeButton.MouseLeftButtonUp += RemoveAction(c);
        activeButton.MouseLeftButtonUp += ActiveAction(c);

        TextBlock nameTextBlock = new()
        {
            Text = c.Name,
            FontSize = 18,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC")),
            TextAlignment = TextAlignment.Center,
        };

        StackPanel buttonPanel = new()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                editButton,
                ascendButton,
            },
        };

        StackPanel textPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Children = { nameTextBlock },
        };
        StackPanel buttonPanel2 = new()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                activeButton,
                removeButton,
            },
        };

        Grid.SetColumn(buttonPanel, 0);
        Grid.SetColumn(textPanel, 1);
        Grid.SetColumn(buttonPanel2, 2);

        grid.Children.Add(buttonPanel);
        grid.Children.Add(textPanel);
        grid.Children.Add(buttonPanel2);

        return grid;
    }

    private static MouseButtonEventHandler RemoveAction(Character c)
    {
        MouseButtonEventHandler handler = (sender, e) =>
        {
            c.Deleted = true;
            c.Activated = false;
        };
        return handler;
    }

    private static MouseButtonEventHandler ActiveAction(Character c)
    {
        MouseButtonEventHandler handler = (sender, e) =>
        {
            c.Activated = !c.Activated;
        };
        return handler;
    }

    private static Image CreateImage(string def, string mouseOver, int width, int height)
    {
        Image i = new()
        {
            Width = width,
            Height = height,
            Source = imageDictionaryTools[def],
        };

        i.MouseEnter += (sender, e) =>
        {
            Image b = (Image)sender;
            b.Source = imageDictionaryTools[mouseOver];
        };

        i.MouseLeave += (sender, e) =>
        {
            Image b = (Image)sender;
            b.Source = imageDictionaryTools[def];
        };

        return i;
    }

    protected override void OnClosed(EventArgs e)
    {
        DataIO.Export(inventory, AllCharactersList);
        base.OnClosed(e);
    }

    private static WrapPanel CreateToolsPanel()
    {
        WrapPanel panel = new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        ScrollViewer scrollViewer = new()
        {
            Content = CreateCharactersPanel()
        };

        Popup popup = new()
        {
            Child = scrollViewer,
            Placement = PlacementMode.Center,
            Width = 400,
            Height = 200
        };

        Button addCharacterButton = new()
        {
            Content = "Add Character",
            Width = 150,
            Height = 30,
            Style = new Style(typeof(Button))
            {
                Setters =
                {
                     new Setter(TemplateProperty, ButtonTemplate()),
                },
            },
        };

        addCharacterButton.Click += (sender, e) =>
        {
           popup.PlacementTarget = addCharacterButton;
           popup.IsOpen = !popup.IsOpen;
        };

        Button manageInventoryButton = new()
        {
            Content = "Manage Inventory",
            Width = 150,
            Height = 30,

            Style = new Style(typeof(Button))
            {
                Setters =
                {
                     new Setter(TemplateProperty, ButtonTemplate()),
                },
            },
        };

        Button managePriorityButton = new()
        {
            Content = "Manage Priority",
            Width = 150,
            Height = 30,
            Style = new Style(typeof(Button))
            {
                Setters =
                {
                     new Setter(TemplateProperty, ButtonTemplate()),
                },
            },
        };

        panel.Children.Add(addCharacterButton);
        panel.Children.Add(manageInventoryButton);
        panel.Children.Add(managePriorityButton);

        return panel;
    }

    private static WrapPanel CreateCharactersPanel()
    {
        WrapPanel panel = new()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(10),
        };

        foreach (var name in AllCharactersList)
        {
            Button btn = new()
            {
                Content = name,
                Width = 100,
                Height = 30,
                Margin = new Thickness(5),
            };

            btn.Click += (s, e) =>
            {
                MessageBox.Show($"Selected character: {name}");
            };

            panel.Children.Add(btn);
        }

        return panel;
    }

    private static ControlTemplate ButtonTemplate()
    {
        ControlTemplate template = new(typeof(Button));

        FrameworkElementFactory border = new(typeof(Border));
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
        border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
        border.SetValue(Border.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")));
        border.SetValue(Border.BorderBrushProperty, Brushes.Black);
        border.SetValue(MarginProperty, new Thickness(2, 2, 2, 2));

        FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter));
        contentPresenter.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentPresenter.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

        border.AppendChild(contentPresenter);

        template.VisualTree = border;

        return template;
    }

    private static LinearGradientBrush SetBackgroundRarity(int rarity)
    {
        var gradient = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1),
            GradientStops = rarity switch
            {
                5 =>
                    [
                        new GradientStop(Color.FromArgb(144, 105, 84, 83), 0),
                        new GradientStop(Color.FromArgb(144, 161, 112, 78), 0.39),
                        new GradientStop(Color.FromArgb(144, 228, 171, 82), 1)
                    ],
                4 =>
                    [
                        new GradientStop(Color.FromArgb(144, 89, 84, 130), 0),
                        new GradientStop(Color.FromArgb(144, 120, 102, 157), 0.39),
                        new GradientStop(Color.FromArgb(144, 183, 133, 201), 1)
                ],
                3 =>
                [
                        new GradientStop(Color.FromArgb(144, 81, 84, 116), 0),
                        new GradientStop(Color.FromArgb(144, 80, 104, 135), 0.39),
                        new GradientStop(Color.FromArgb(144, 75, 160, 180), 1),
                ],
                2 =>
                [
                        new GradientStop(Color.FromArgb(144, 72, 87, 92), 0),
                        new GradientStop(Color.FromArgb(144, 72, 107, 103), 0.39),
                        new GradientStop(Color.FromArgb(144, 98, 152, 113), 1),
                ],
                _ =>
                [
                        new GradientStop(Color.FromArgb(144, 79, 88, 100), 0),
                        new GradientStop(Color.FromArgb(144, 95, 102, 115), 0.39),
                        new GradientStop(Color.FromArgb(144, 135, 147, 156), 1),
                ],
            },
        };
        return gradient;
    }

    private static Style ScrollStyle()
    {
        Style style = new(typeof(ScrollBar));

        Trigger trigger = new()
        {
            Property = ScrollBar.OrientationProperty,
            Value = Orientation.Vertical,

            Setters =
            {
                new Setter (WidthProperty, 20.0),
                new Setter (TemplateProperty, ScrollBarTemplate()),
            },
        };

        style.Triggers.Add(trigger);
        return style;
    }

    private static ControlTemplate ScrollBarTemplate()
    {
        string xamlCode = @"
<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
                xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
                xmlns:local=""clr-namespace:Genshin_Calculator""
                x:Key=""VerticalScrollBar"" TargetType=""ScrollBar"">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""*""/>
        </Grid.RowDefinitions>
        <Border Grid.Row=""1"" Width=""10.0"" Background=""#0f0f1e""></Border>
        <Track Name=""PART_Track"" Grid.Row=""1"" IsDirectionReversed=""True"">
            <Track.Thumb>
                <Thumb>
                    <Thumb.Style>
                        <Style x:Key=""ScrollBar_Thumb"" TargetType=""Thumb"">
                            <Setter Property=""SnapsToDevicePixels"" Value=""True""/>
                            <Setter Property=""OverridesDefaultStyle"" Value=""True""/>
                            <Setter Property=""Width"" Value=""10.0""/>
                            <Setter Property=""Template"">
                                <Setter.Value>
                                    <ControlTemplate TargetType=""Thumb"">
                                        <Border x:Name=""border"" Background=""#0272c8"" SnapsToDevicePixels=""True""></Border>
                                        <ControlTemplate.Triggers>
                                            <Trigger Property=""IsMouseOver"" Value=""True"">
                                                <Setter Property=""Background"" TargetName=""border"" Value=""#0a0c33""/>
                                                <Setter Property=""BorderBrush"" TargetName=""border"" Value=""#0a0c33""/>
                                            </Trigger>
                                        </ControlTemplate.Triggers>
                                    </ControlTemplate>
                                </Setter.Value>
                            </Setter>
                        </Style>
                    </Thumb.Style>
                </Thumb>
            </Track.Thumb>
        </Track>
    </Grid>
</ControlTemplate>";

        return (ControlTemplate)XamlReader.Parse(xamlCode);
    }
}