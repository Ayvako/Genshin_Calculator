using Genshin.src;
using Genshin.src.LevelingResources;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Resources;
using System.Windows.Data;

namespace Genshin_Calculator
{
    public partial class MainWindow : Window
    {
        private static List<Character> CharactersList;
        private static Dictionary<string, ImageSource> ImageDictionaryCharacter;
        private static Dictionary<string, ImageSource> ImageDictionaryMaterial;
        private static Dictionary<string, ImageSource> ImageDictionaryTools;

        public MainWindow()
        {
            InitializeComponent();

            DataIO.Import();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MinHeight = 500;
            MinWidth = 500;
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
            ImageDictionaryCharacter = LoadCharacterImages(CharactersList);
            ImageDictionaryMaterial = LoadMaterialImages(Inventory.MyInventory);
            ImageDictionaryTools = LoadToolsImages();

            Content = MainPanel();

        }



        private static ScrollViewer MainPanel()
        {

            StackPanel panel = new()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(CreateToolsPanel());
            panel.Children.Add(CreateFarmPanel(CharactersList));

            ScrollViewer scrollViewer = new ()
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

        private static WrapPanel CreateResourcesPanel(List<Genshin.src.LevelingResources.Material> materials)
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
                    Source = ImageDictionaryMaterial[m.Name],
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

        private static StackPanel CreateFarmPanel(List<Character> characters)
        {
            WrapPanel wrapPanel = new();
            var mat = Inventory.CalcRequiredMaterials();
            foreach (var c in characters)
            {
                Grid tableContents = CreateTableContentsPanel(c);
                Grid infoGrid = CreateInfoPanel(c);
                WrapPanel resourcesPanel = CreateResourcesPanel(mat[c]);

                StackPanel characterPanel = new()
                {

                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        tableContents,
                        infoGrid,
                        resourcesPanel
                    }

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

            return new StackPanel()
            {
                Orientation = Orientation.Vertical,

                Margin = new Thickness(5),
                Children = { wrapPanel }
            };
        }

        private static Grid CreateInfoPanel(Character c)
        {

            Image avatar = new()
            {
                Width = 128,
                Height = 128,
                Source = ImageDictionaryCharacter[c.Name],
            };
            WrapPanel avatarPanel = new()
            {
                Children = { avatar },
                Background = SetBackgroundRarity(c.Assets.Rarity)
            };
            Border avatarBorder = new()
            {
                Width = 128,
                Height = 128,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                Child = avatarPanel

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
                        levelUpTextBlock
                    }
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
                        talentsLevelUpTextBlock
                    }
            };
            StackPanel statsPanel = new()
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


            return infoGrid;
        }

        private static Grid CreateTableContentsPanel(Character c)
        {
            Grid grid = new() { Background = SetBackgroundRarity(c.Assets.Rarity) };

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            // Создание Button и TextBlock

            Image editButton   = CreateImage("edit_icon_default.png", "edit_icon_mouseover.png", 40, 40);
            Image ascendButton = CreateImage("acsend_icon_default.png", "acsend_icon_mouseover.png", 40, 40);
            Image activeButton = CreateImage("active_icon_default.png", "active_icon_mouseover.png", 40, 40);
            Image removeButton = CreateImage("remove_icon_default.png", "remove_icon_mouseover.png", 40, 40);

            
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

            StackPanel textPanel = new()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children = { nameTextBlock }
            };
            StackPanel buttonPanel2 = new()
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    activeButton,
                    removeButton
                }
            };

            Grid.SetColumn(buttonPanel, 0); 
            Grid.SetColumn(textPanel, 1); 
            Grid.SetColumn(buttonPanel2, 2); 

            grid.Children.Add(buttonPanel);
            grid.Children.Add(textPanel);
            grid.Children.Add(buttonPanel2);

            return grid;
        }

        private static Image CreateImage(string def, string mouseOver, int width, int height)
        {
            Image i = new ()
            {
                Width = width,
                Height = height,
                Source = ImageDictionaryTools[def],
                
            };

            i.MouseEnter += (sender, e) =>
            {
                Image b = (Image)sender;
                b.Source = ImageDictionaryTools[mouseOver];
            };

            i.MouseLeave += (sender, e) =>
            {
                Image b = (Image)sender;
                b.Source = ImageDictionaryTools[def];
            };

            return i;
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
            ControlTemplate template = new (typeof(Button));

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
            Dictionary<string, ImageSource> imageDictionary = new();

            foreach (var character in characters)
            {
                if (!imageDictionary.ContainsKey(character.Name))
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.DecodePixelWidth = 128;
                    bitmapImage.DecodePixelHeight = 128;
                    bitmapImage.UriSource = new Uri($"/Resources/Image/Characters/{character.Name}.png", UriKind.Relative);
                    bitmapImage.EndInit();

                    imageDictionary[character.Name] = bitmapImage;
                }
            }

            return imageDictionary;
        }

        private static Dictionary<string, ImageSource> LoadMaterialImages(Dictionary<string, int> dict)
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
                    bitmapImage.DecodePixelHeight = 64;

                    bitmapImage.EndInit();

                    imageDictionary[material] = bitmapImage;
                }
            }

            return imageDictionary;
        }
        
        private Dictionary<string, ImageSource> LoadToolsImages()
        {

            var fileNames = GetResourcesUnder("Resources/Image/Tools");

            Dictionary<string, ImageSource> imageDictionary = new();

            foreach(var f in fileNames)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.DecodePixelHeight = 40;
                bitmapImage.DecodePixelWidth = 40;

                bitmapImage.UriSource = new Uri($"pack://application:,,,/Resources/Image/Tools/{f}");
                bitmapImage.EndInit();

                imageDictionary[f] = bitmapImage;
            }

            return imageDictionary;
        }

        private static string[] GetResourcesUnder(string folder)
        {
            folder = folder.ToLower() + "/";

            var assembly = Assembly.GetCallingAssembly();
            var resourcesName = assembly.GetName().Name + ".g.resources";
            var stream = assembly.GetManifestResourceStream(resourcesName);
            var resourceReader = new ResourceReader(stream);

            var resources =
                from p in resourceReader.OfType<DictionaryEntry>()
                let theme = (string)p.Key
                where theme.StartsWith(folder)
                select theme.Substring(folder.Length);

            return resources.ToArray();
        }

        private static Style ScrollStyle()
        {
            Style style = new (typeof(ScrollBar));


            Trigger trigger = new ()
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
}