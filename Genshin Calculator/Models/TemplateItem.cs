using Genshin_Calculator.Models.Enums;

namespace Genshin_Calculator.Models;

public class TemplateItem
{
    public MaterialTypes Type { get; set; }

    public MaterialRarity Rarity { get; set; }

    public int Amount { get; set; }
}