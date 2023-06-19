using System;
using System.Collections.Generic;

namespace Genshin.src.LevelingResources
{
    public static class Enemy
    {
        private static readonly Dictionary<string, string[]> Enemies = DataIO.GetMaterials("Data/Enemies.json");

        public static string GetMaterial(Character character, string rarity) => rarity switch
        {
            "white" => Enemies[character.Assets.Enemy][0],
            "green" => Enemies[character.Assets.Enemy][1],
            "blue"  => Enemies[character.Assets.Enemy][2],
            _ => throw new Exception("Unknown Property Name"),
        };
    }
}