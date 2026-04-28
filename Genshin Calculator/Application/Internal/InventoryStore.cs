using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Application.Internal;

internal sealed class InventoryStore
{
    public Inventory Inventory { get; set; } = new Inventory();
}