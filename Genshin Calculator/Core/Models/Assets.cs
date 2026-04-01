using System;
using Genshin_Calculator.Core.Helpers;
using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Core.Models;

public record Assets
{
    public Assets(string name, WeaponType weapon, Element element, string localSpecialty, string skillMaterials, string enemy, string miniBoss, string weeklyBoss, MaterialRarity rarity)
    {
        this.Name = name;
        this.LocalSpecialty = localSpecialty;
        this.SkillMaterials = skillMaterials;
        this.Element = element;
        this.Weapon = weapon;
        this.Enemy = enemy;
        this.MiniBoss = miniBoss;
        this.WeeklyBoss = weeklyBoss;
        this.Rarity = rarity;
    }

    public string Name { get; set; }

    public Uri ImagePath => ResourcePaths.Character(this.Name);

    public string LocalSpecialty { get; set; }

    public Element Element { get; set; }

    public WeaponType Weapon { get; set; }

    public string Enemy { get; set; }

    public string MiniBoss { get; set; }

    public string WeeklyBoss { get; set; }

    public string SkillMaterials { get; set; }

    public MaterialRarity Rarity { get; set; }
}