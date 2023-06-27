using Genshin.src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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
                        CreateRequiredMaterialsPanel(CharactersList)
                    }
            };

        }





        private static ScrollViewer CreateRequiredMaterialsPanel(List<Character> characters)
        {
            WrapPanel wrapPanel = new WrapPanel();
            var mat = Inventory.CalcRequiredMaterials();

            foreach (var c in characters)
            {
                TextBlock nameTextBlock = new TextBlock()
                {
                    Text = c.Name,
                    FontSize = 18,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC")),
                    TextAlignment = TextAlignment.Center,
                    Background = new LinearGradientBrush()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = 
                        {
                            new GradientStop(Color.FromArgb(144, 105, 84, 83), 0),
                            new GradientStop(Color.FromArgb(144, 161, 112, 78), 0.39),
                            new GradientStop(Color.FromArgb(144, 228, 171, 82), 1)


                        }
                    }

                };

                Image avatar = new()
                {
                    Width = 128,
                    Height = 128,
                    Source = imageDictionary[c.Name],
                };
                WrapPanel avatarPanel = new WrapPanel()
                {
                    Children = { avatar },
                    
                    Background = new LinearGradientBrush()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops =
                        {
                            new GradientStop(Color.FromArgb(144, 105, 84, 83), 0),
                            new GradientStop(Color.FromArgb(144, 161, 112, 78), 0.39),
                            new GradientStop(Color.FromArgb(144, 228, 171, 82), 1)
                        }
                    }
                };

                Border avatarBorder = new Border()
                {
                    Width = 128,
                    Height = 128,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Child = avatarPanel
                    
                };


                TextBlock levelTextBlock = new TextBlock()
                {
                    Text = "Levels",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,


                };
                TextBlock levelUpTextBlock = new TextBlock()
                {
                    Text = $"{c.CurrentLevel} -> {c.DesiredLevel}",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC")),
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 14,

                };

                StackPanel levelPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        levelTextBlock,
                        levelUpTextBlock
                    }
                };

                TextBlock talentTextBlock = new TextBlock()
                {
                    Text = "Talents",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,

                };
                TextBlock talentsLevelUpTextBlock = new TextBlock()
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


                StackPanel talentsPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        talentTextBlock,
                        talentsLevelUpTextBlock
                    }
                };

                StackPanel statsPanel = new StackPanel()
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

                Grid infoGrid = new () 
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


                WrapPanel resourcesPanel = new WrapPanel();


                foreach (var m in mat[c])
                {

                    Image materialImage = new()
                    {
                        Source = imageDictionaryMaterial[m.Name],
                        Stretch = Stretch.None,


                    };
                    Debug.WriteLine(m.Name);
                    resourcesPanel.Children.Add(materialImage);
                }


                StackPanel characterPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        nameTextBlock,
                        infoGrid,
                        resourcesPanel
                    }

                };

                Border characterBorder = new Border()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = characterPanel,
                    MaxWidth = 712,
                    MinWidth = 350

                };


                wrapPanel.Children.Add(characterBorder);
            }

            return new ScrollViewer()
            {
                Content = wrapPanel
            };
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

        private static Dictionary<string, ImageSource> LoadCharacterImages(List<Character> characters)
        {
            Dictionary<string, ImageSource> imageDictionary = new ();

            foreach (var character in characters)
            {
                if (!imageDictionary.ContainsKey(character.Name))
                {
                    BitmapImage bitmapImage = new BitmapImage();
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
