using System;
using System.Collections.Generic;
using Genshin_Calculator.ProjectRoot.Src.Services;
using Genshin_Calculator.ProjectRoot.Src.Models;

namespace Genshin_Calculator.ProjectRoot.src.LevelingResources;

public static class Book
{
    private static readonly Dictionary<string, string[]> Books = DataIO.GetMaterials("Books");

    public static string GetMaterial(Character character, string rarity) => rarity switch
    {
        "green" => Books[character.Assets.BookType][0],
        "blue" => Books[character.Assets.BookType][1],
        "violet" => Books[character.Assets.BookType][2],
        _ => throw new Exception("Unknown Property Name"),
    };
}
