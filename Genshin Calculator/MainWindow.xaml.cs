using Genshin.src;
using Genshin.src.LevelingResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;


namespace Genshin_Calculator
{
    public partial class MainWindow : Window
    {
        private static List<Character> CharactersList;
        private static Dictionary<string, ImageSource> imageDictionary;
        private static Dictionary<string, ImageSource> imageDictionaryMaterial;

        public MainWindow()
        {
            InitializeComponent();

            DataIO.Import();


            Background = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops =
                {
                    new GradientStop((Color)ColorConverter.ConvertFromString("#31394E"), 0),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#1D212D"), 0.2),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#191E33"), 0.4),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#141829"), 0.6),
                    new GradientStop((Color)ColorConverter.ConvertFromString("#101321"), 1)
                }
            };

            CharactersList = Inventory.GetActiveCharacters();
            imageDictionary = LoadCharacterImages(CharactersList);
            imageDictionaryMaterial = LoadMaterialImages(Inventory.MyInventory);

  /*          WrapPanel charactersPanel = new ();

            foreach (var c in CharactersList)
            {
                StackPanel stackPanel = new ();

                Image image = new ()
                {
                    Source = imageDictionary[c.Name],
                    Stretch = Stretch.None
                };



                TextBlock textBlock = new ()
                {
                    Text = c.Name,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);

                charactersPanel.Children.Add(stackPanel);
            }*/


            Content = MainPanel();

        }

        private static StackPanel MainPanel()
        {
            
            return new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Children =
                    {
                        CreateToolsPanel(),
                        CreateFarmPanel(CharactersList)
                    }
            };

        }








        private static WrapPanel CreateResourcesPanel(List<Material> materials)
        {
            WrapPanel resourcesPanel = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5),

            };


            foreach (var m in materials)
            {
                StackPanel amountPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                };

                TextBlock amountText = new TextBlock()
                {
                    Foreground = Brushes.White,
                    Text = m.Amount.ToString(),
                    TextAlignment = TextAlignment.Center,
                };


                Border amountBorder = new Border()
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

                Border materialBorder = new Border()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = amountPanel,
                    CornerRadius = new CornerRadius(10),

                    Background = SetBackgroundRarity(m.Rarity)


                };

                resourcesPanel.Children.Add(materialBorder);
            }

            return resourcesPanel;
        }

        private static ScrollViewer CreateFarmPanel(List<Character> characters)
        {
            WrapPanel wrapPanel = new ();
            var mat = Inventory.CalcRequiredMaterials();
            foreach (var c in characters)
            {
                Grid tableContents = CreateTableContentsPanel(c);

                Image avatar = new()
                {
                    Width = 128,
                    Height = 128,
                    Source = imageDictionary[c.Name],
                };
                WrapPanel avatarPanel = new ()
                {
                    Children = { avatar },
                    Background = SetBackgroundRarity(c.Assets.Rarity)
                };
                Border avatarBorder = new ()
                {
                    Width = 128,
                    Height = 128,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Child = avatarPanel

                };
                TextBlock levelTextBlock = new ()
                {
                    Text = "Levels",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                };
                TextBlock levelUpTextBlock = new ()
                {
                    Text = $"{c.CurrentLevel} -> {c.DesiredLevel}",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC")),
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 14,

                };
                StackPanel levelPanel = new ()
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        levelTextBlock,
                        levelUpTextBlock
                    }
                };
                TextBlock talentTextBlock = new ()
                {
                    Text = "Talents",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,

                };
                TextBlock talentsLevelUpTextBlock = new ()
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
                StackPanel talentsPanel = new ()
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        talentTextBlock,
                        talentsLevelUpTextBlock
                    }
                };
                StackPanel statsPanel = new ()
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        levelPanel,
                        talentsPanel
                    }
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
                            Width = new GridLength(1, GridUnitType.Star)
                        }
                    },
                    Children =
                    {
                        avatarBorder,
                        statsPanel
                    }

                };

                WrapPanel resourcesPanel = CreateResourcesPanel(mat[c]);

                StackPanel characterPanel = new ()
                {

                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        tableContents,
                        infoGrid,
                        resourcesPanel
                    }

                };

                Border characterBorder = new ()
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
                            new GradientStop(Color.FromRgb(16, 19, 33), 0.8)
                        }
                    },
                    Effect = new DropShadowEffect()
                    {
                        Color = Colors.Black,
                        Direction = 0,
                        ShadowDepth = 0,
                        BlurRadius = 11,
                        Opacity = 0.75,
                    }
                };

                wrapPanel.Children.Add(characterBorder);
            }

            return new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Margin = new Thickness(5),
                Content = wrapPanel
            };
        }

        private static Grid CreateTableContentsPanel(Character c)
        {
            Grid grid = new() { Background = SetBackgroundRarity(c.Assets.Rarity) };

            // Определение столбцов с нужными ширинами
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) }); // Левый столбец
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // Центральный столбец
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) }); // Правый столбец

            // Создание Button и TextBlock
            Button editButton = new() { Content = "B1" };
            Button ascendButton = new() { Content = "B2" };
            Button activeButton = new() { Content = "B3" };
            Button removeButton = new() { Content = "B4" };
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
                    ascendButton
                }
            };

            StackPanel textPanel = new () 
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment= HorizontalAlignment.Center,
                VerticalAlignment=VerticalAlignment.Center,
                Children = {nameTextBlock}
            };
            StackPanel buttonPanel2 = new () 
            { 
                Orientation = Orientation.Horizontal,
                Children =
                {
                    activeButton,
                    removeButton
                }
            };

            // Размещение элементов в сетке
            Grid.SetColumn(buttonPanel, 0); // Левый столбец
            Grid.SetColumn(textPanel, 1); // Центральный столбец
            Grid.SetColumn(buttonPanel2, 2); // Правый столбец

            // Добавление элементов в сетку
            grid.Children.Add(buttonPanel);
            grid.Children.Add(textPanel);
            grid.Children.Add(buttonPanel2);

            return grid;
        }


        private static WrapPanel CreateToolsPanel()
        {
            WrapPanel panel = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            //ScrollViewer scrollViewer = new()
            //{
            //    Content = charactersPanel
            //};
            //Popup popup = new()
            //{
            //    Child = scrollViewer,
            //    Placement = PlacementMode.Center,
            //    Width = 400,
            //    Height = 200
            //};

            Button addCharacterButton = new()
            {
                Content = "Add Character",
                Width = 150,
                Height = 30,
                Style = new Style(typeof(Button))
                {
                    Setters =
                    {
                         new Setter(TemplateProperty, ButtonTemplate())
                    }
                }
            };
            

            //addCharacterButton.Click += (sender, e) =>
            //{
            //    
            //   popup.PlacementTarget = addCharacterButton;
            //   popup.IsOpen = !popup.IsOpen;
            //    
            //};

            Button manageInventoryButton = new()
            {
                Content = "Manage Inventory",
                Width = 150,
                Height = 30,

                Style = new Style(typeof(Button))
                {
                    Setters =
                    {
                         new Setter(TemplateProperty, ButtonTemplate())
                    }
                }
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
                         new Setter(TemplateProperty, ButtonTemplate())
                    }

                }
            };

            panel.Children.Add(addCharacterButton);
            panel.Children.Add(manageInventoryButton);
            panel.Children.Add(managePriorityButton);

            return panel;
        }

        private static ControlTemplate ButtonTemplate()
        {
            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory border = new (typeof(Border));
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
                    5 => new GradientStopCollection
                        {
                            new GradientStop(Color.FromArgb(144, 105, 84, 83), 0),
                            new GradientStop(Color.FromArgb(144, 161, 112, 78), 0.39),
                            new GradientStop(Color.FromArgb(144, 228, 171, 82), 1)
                        },
                    4 => new GradientStopCollection
                        {
                            new GradientStop(Color.FromArgb(144, 89, 84, 130), 0),
                            new GradientStop(Color.FromArgb(144, 120, 102, 157), 0.39),
                            new GradientStop(Color.FromArgb(144, 183, 133, 201), 1)
                    },
                    3 => new GradientStopCollection
                    {
                            new GradientStop(Color.FromArgb(144, 81, 84, 116), 0),
                            new GradientStop(Color.FromArgb(144, 80, 104, 135), 0.39),
                            new GradientStop(Color.FromArgb(144, 75, 160, 180), 1),
                    },
                    2 => new GradientStopCollection
                    {
                            new GradientStop(Color.FromArgb(144, 72, 87, 92), 0),
                            new GradientStop(Color.FromArgb(144, 72, 107, 103), 0.39),
                            new GradientStop(Color.FromArgb(144, 98, 152, 113), 1),
                    },
                    _ => new GradientStopCollection
                    {
                            new GradientStop(Color.FromArgb(144, 79, 88, 100), 0),
                            new GradientStop(Color.FromArgb(144, 95, 102, 115), 0.39),
                            new GradientStop(Color.FromArgb(144, 135, 147, 156), 1),
                    },
                }
            };
            return gradient;

        }

        private static Dictionary<string, ImageSource> LoadCharacterImages(List<Character> characters)
        {
            Dictionary<string, ImageSource> imageDictionary = new ();

            foreach (var character in characters)
            {
                if (!imageDictionary.ContainsKey(character.Name))
                {
                    BitmapImage bitmapImage = new ();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri($"/Resources/Image/Characters/{character.Name}.png", UriKind.Relative);
                    bitmapImage.EndInit();

                    imageDictionary[character.Name] = bitmapImage;
                }
            }

            return imageDictionary;
        }

        private static Dictionary<string, ImageSource> LoadMaterialImages(Dictionary<string,int> dict)
        {
            Dictionary<string, ImageSource> imageDictionary = new();

            foreach (var material in dict.Keys)
            {
                if (!imageDictionary.ContainsKey(material))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri($"/Resources/Image/Materials/{material}.png", UriKind.Relative);
                    bitmapImage.DecodePixelWidth = 64;
                    bitmapImage.EndInit();

                    imageDictionary[material] = bitmapImage;
                }
            }

            return imageDictionary;
        }
    }
}