using Genshin_Calculator.Models;
using Genshin_Calculator.Services.Interfaces;

namespace Genshin_Calculator.Services;

public sealed class InventoryStore : IInventoryStore
{
    public Inventory Inventory { get; set; } = new Inventory();
}