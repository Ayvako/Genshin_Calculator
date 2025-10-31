using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Genshin_Calculator.ProjectRoot.src.LevelingResources;
using Genshin_Calculator.ProjectRoot.Src.Models;

namespace Genshin_Calculator.ProjectRoot.Src.Services
{
    public class ImageService
    {
        public static Dictionary<string, ImageSource> LoadCharacterImages(List<Character> characters)
        {
            Dictionary<string, ImageSource> imageDictionary = [];

            foreach (var character in characters)
            {
                if (!imageDictionary.ContainsKey(character.Name))
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.DecodePixelWidth = 128;
                    bitmapImage.DecodePixelHeight = 128;
                    bitmapImage.UriSource = new Uri($"ProjectRoot/Resources/Images/Characters/{character.Name}.png", UriKind.Relative);
                    bitmapImage.EndInit();

                    imageDictionary[character.Name] = bitmapImage;
                }
            }

            return imageDictionary;
        }

        public static Dictionary<string, ImageSource> LoadMaterialImages(List<Material> dict)
        {
            Dictionary<string, ImageSource> imageDictionary = [];

            foreach (var material in dict)
            {
                if (!imageDictionary.ContainsKey(material.Name))
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri($"ProjectRoot/Resources/Images/Materials/{material}.png", UriKind.Relative);
                    bitmapImage.DecodePixelWidth = 64;
                    bitmapImage.DecodePixelHeight = 64;

                    bitmapImage.EndInit();

                    imageDictionary[material.Name] = bitmapImage;
                }
            }

            return imageDictionary;
        }

        public static Dictionary<string, ImageSource> LoadToolsImages()
        {
            var fileNames = GetResourcesUnder("ProjectRoot/Resources/Images/Tools");

            Dictionary<string, ImageSource> imageDictionary = [];

            foreach (var f in fileNames)
            {
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.DecodePixelHeight = 30;
                bitmapImage.DecodePixelWidth = 30;

                bitmapImage.UriSource = new Uri($"pack://application:,,,/ProjectRoot/Resources/Images/Tools/{f}");
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
                select theme[folder.Length..];

            return [.. resources];
        }
    }
}
