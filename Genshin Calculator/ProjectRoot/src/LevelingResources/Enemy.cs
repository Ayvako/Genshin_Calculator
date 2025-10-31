using System;
using System.Collections.Generic;
using Genshin_Calculator.ProjectRoot.Src.Models;
using Genshin_Calculator.ProjectRoot.Src.Services;

namespace Genshin_Calculator.ProjectRoot.Src.LevelingResources
{
    public static class Enemy
    {
        private static readonly Dictionary<string, string[]> Enemies = DataIO.GetMaterials("Enemies");

        public static string GetMaterial(Character character, string rarity) => rarity switch
        {
            "white" => Enemies[character.Assets.Enemy][0],
            "green" => Enemies[character.Assets.Enemy][1],
            "blue" => Enemies[character.Assets.Enemy][2],
            _ => throw new Exception("Unknown Property Name"),
        };
    }
}