using System.Collections.Generic;

namespace Genshin_Calculator.Core.Models;

public class LevelData
{
    public Dictionary<string, int> BaseCosts { get; set; } = [];

    public Dictionary<string, List<TemplateItem>> AscensionCosts { get; set; } = [];
}