using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services
{
    public static class ImageService
    {
        public static void InitializeFromConfig()
        {
            var basePath = App.Configuration["Resources:ImagesBasePath"];
            ArgumentNullException.ThrowIfNull(basePath);
            ResourcePaths.SetBasePath(basePath);
        }

        public static Dictionary<string, ImageSource> LoadCharacterImages(List<Character> characters)
        {
            return characters
                .Select(c => c.Name)
                .Distinct()
                .ToDictionary(
                    name => name,
                    name =>
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.BeginInit();
                        bitmapImage.DecodePixelWidth = 128;
                        bitmapImage.DecodePixelHeight = 128;
                        bitmapImage.UriSource = new Uri($"{ResourcePaths.Characters}/{name}.png");
                        bitmapImage.EndInit();
                        return (ImageSource)bitmapImage;
                    });
        }

        public static string GetCharacterPath(string name)
        {
            return $"{ResourcePaths.Characters}/{name}.png";
        }

        public static Dictionary<string, ImageSource> LoadMaterialImages(Inventory dict)
        {
            return dict.Materials
                .Select(m => m.Name)
                .Distinct()
                .ToDictionary(
                    name => name,
                    name =>
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri($"{ResourcePaths.Materials}/{name}.png");
                        bitmapImage.DecodePixelWidth = 64;
                        bitmapImage.DecodePixelHeight = 64;
                        bitmapImage.EndInit();
                        return (ImageSource)bitmapImage;
                    });
        }

        public static Dictionary<string, ImageSource> LoadToolsImages()
        {
            var fileNames = GetResourcesUnder($"{ResourcePaths.Tools}");

            Dictionary<string, ImageSource> imageDictionary = [];

            foreach (var name in fileNames)
            {
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.DecodePixelHeight = 30;
                bitmapImage.DecodePixelWidth = 30;

                bitmapImage.UriSource = new Uri($"{ResourcePaths.Tools}/{name}");
                bitmapImage.EndInit();

                imageDictionary[name] = bitmapImage;
            }

            return imageDictionary;
        }

        public static ImageSource GetCharacterImage(string name)
        {
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.DecodePixelWidth = 128;
            bitmapImage.DecodePixelHeight = 128;
            bitmapImage.UriSource = new Uri($"{ResourcePaths.Characters}/{name}.png");
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static ImageSource GetMaterialImage(string name)
        {
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri($"{ResourcePaths.Materials}/{name}.png", UriKind.Relative);
            bitmapImage.DecodePixelWidth = 64;
            bitmapImage.DecodePixelHeight = 64;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private static string[] GetResourcesUnder(string folder)
        {
            folder = folder.ToLower() + "/";

            var assembly = Assembly.GetCallingAssembly();
            var resourcesName = assembly.GetName().Name + ".g.resources";
            var stream = assembly.GetManifestResourceStream(resourcesName);
            ArgumentNullException.ThrowIfNull(stream);
            var resourceReader = new ResourceReader(stream);

            var resources =
                from p in resourceReader.OfType<DictionaryEntry>()
                let theme = (string)p.Key
                where theme.StartsWith(folder)
                select theme[folder.Length..];

            return [.. resources];
        }
    }
}