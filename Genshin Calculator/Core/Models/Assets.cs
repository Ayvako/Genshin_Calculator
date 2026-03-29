using System;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Core.Models;

public record Assets
{
    public Assets(string name, WeaponType weapon, Element element, string localSpecialty, string skillMaterials, string enemy, string miniBoss, string weeklyBoss, MaterialRarity rarity)
    {
        Name = name;
        LocalSpecialty = localSpecialty;
        SkillMaterials = skillMaterials;
        Element = element;
        Weapon = weapon;
        Enemy = enemy;
        MiniBoss = miniBoss;
        WeeklyBoss = weeklyBoss;
        Rarity = rarity;
    }

    public string Name { get; set; }

    public Uri ImagePath => ResourcePaths.Character(Name);

    public string LocalSpecialty { get; set; }

    public Element Element { get; set; }

    public WeaponType Weapon { get; set; }

    public string Enemy { get; set; }

    public string MiniBoss { get; set; }

    public string WeeklyBoss { get; set; }

    public string SkillMaterials { get; set; }

    public MaterialRarity Rarity { get; set; }
}