using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Resources;

namespace Genshin.src.LevelingResources
{
    public static class Book
    {

        private static readonly Dictionary<string, string[]> Books = DataIO.GetMaterials("Books");






        public static string GetMaterial(Character character, string rarity) => rarity switch
        {
            "green"  => Books[character.Assets.BookType][0],
            "blue"   => Books[character.Assets.BookType][1],
            "violet" => Books[character.Assets.BookType][2],
            _ => throw new Exception("Unknown Property Name"),
        };
    }
}
