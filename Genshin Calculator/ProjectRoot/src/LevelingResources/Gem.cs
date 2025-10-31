using System;
using System.Collections.Generic;
using Genshin_Calculator.ProjectRoot.Src.Models;
using Genshin_Calculator.ProjectRoot.Src.Services;

namespace Genshin_Calculator.ProjectRoot.Src.LevelingResources
{
    public static class Gem
    {
        private static readonly Dictionary<string, string[]> Gems = DataIO.GetMaterials("Gems");

        public static string GetMaterial(Character character, string rarity) => rarity switch
        {
            "green" => Gems[character.Assets.Element][0],
            "blue" => Gems[character.Assets.Element][1],
            "violet" => Gems[character.Assets.Element][2],
            "orange" => Gems[character.Assets.Element][3],
            _ => throw new Exception("Unknown Property Name"),
        };
    }
}