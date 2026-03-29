using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Core.Models;

public class TemplateItem
{
    public MaterialTypes Type { get; set; }

    public MaterialRarity Rarity { get; set; }

    public int Amount { get; set; }
}