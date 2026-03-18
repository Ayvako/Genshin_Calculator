using System.Collections.Generic;

namespace Genshin_Calculator.Models;

public class SkillLevelData
{
    public Dictionary<int, List<TemplateItem>> LevelCosts { get; set; } = [];
}