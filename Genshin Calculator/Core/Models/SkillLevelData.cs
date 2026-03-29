using System.Collections.Generic;

namespace Genshin_Calculator.Core.Models;

public class SkillLevelData
{
    public Dictionary<int, List<TemplateItem>> LevelCosts { get; set; } = [];
}