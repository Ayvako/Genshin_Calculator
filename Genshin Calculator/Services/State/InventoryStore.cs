using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.State;

public sealed class InventoryStore : IInventoryStore
{
    public Inventory Inventory { get; set; } = new Inventory();
}