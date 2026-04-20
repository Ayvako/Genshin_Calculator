using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Presentation.Features.Inventory;

public class FilterOption
{
    public MaterialTypes? Value { get; set; }

    public override string ToString()
    => this.Value?.ToString() ?? "Показать всё";
}