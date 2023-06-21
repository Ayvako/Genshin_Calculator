using Genshin.src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Genshin_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Character> CharactersList;

        public MainWindow()
        {
            InitializeComponent();

            DataIO.Import();

            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);

            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#31394E"), 0));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#1D212D"), 0.2));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#191E33"), 0.4));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#141829"), 0.6));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#101321"), 1));

            Background = gradientBrush;

            CharactersList = Inventory.GetDeletedCharacters();
            Dictionary<string, ImageSource> imageDictionary = LoadCharacterImages(CharactersList);

            WrapPanel charactersPanel = new ();

            foreach (var c in CharactersList)
            {
                StackPanel stackPanel = new ();

                Image image = new ()
                {
                    Source = imageDictionary[c.Name],
                    Stretch = Stretch.None
                };

                Button button = new ()
                {
                    Content = image,
                    Margin = new Thickness(5)
                };
                button.Click += ButtonClick;

                TextBlock textBlock = new ()
                {
                    Text = c.Name,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                stackPanel.Children.Add(button);
                stackPanel.Children.Add(textBlock);
                stackPanel.MouseLeftButtonUp += ButtonClick;

                charactersPanel.Children.Add(stackPanel);
            }


            Content = InitInvenoryPanel(charactersPanel);
        }


        private static WrapPanel InitInvenoryPanel(WrapPanel charactersPanel)
        {
            WrapPanel panel = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            ScrollViewer scrollViewer = new()
            {
                Content = charactersPanel
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
                         new Setter(TemplateProperty, CreateButtonTemplate())
                    }
                }
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
                         new Setter(TemplateProperty, CreateButtonTemplate())
                    }
                }
            };


            Button managePriorityButton = new()
            {
                Content = "Manage Priority",
                Width = 150,
                Height = 30,
                //Margin = new Thickness(10, 0, 10, 0),

                Style = new Style(typeof(Button))
                {
                    Setters =
                    {
                         new Setter(TemplateProperty, CreateButtonTemplate())
                    }

                }
            };

            panel.Children.Add(addCharacterButton);
            panel.Children.Add(manageInventoryButton);
            panel.Children.Add(managePriorityButton);

            return panel;
        }

        static ControlTemplate CreateButtonTemplate()
        {
            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory border = new (typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            border.SetValue(Border.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFECE5D8")));
            border.SetValue(Border.BorderBrushProperty, Brushes.Black);
            border.SetValue(MarginProperty, new Thickness(2, 2, 2, 2));

            FrameworkElementFactory contentPresenter = new (typeof(ContentPresenter));
            contentPresenter.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);

            template.VisualTree = border;

            return template;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click");
            //Button button = (Button)sender;
        }

        private static Dictionary<string, ImageSource> LoadCharacterImages(List<Character> characters)
        {
            Dictionary<string, ImageSource> imageDictionary = new ();

            foreach (var character in characters)
            {
                // Проверяем, загружена ли картинка для данного персонажа
                if (!imageDictionary.ContainsKey(character.Name))
                {
                    // Если картинка не загружена, то загружаем ее и добавляем в словарь
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri($"/Resources/Image/{character.Name}.png", UriKind.Relative);
                    bitmapImage.DecodePixelWidth = 64;
                    bitmapImage.EndInit();

                    imageDictionary[character.Name] = bitmapImage;
                }
            }

            return imageDictionary;
        }


    }
}
