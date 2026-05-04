using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Application.State;

internal sealed class InventoryStore
{
    public Inventory Inventory { get; set; } = new Inventory();
}