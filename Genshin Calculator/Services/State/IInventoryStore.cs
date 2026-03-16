using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.State;

public interface IInventoryStore
{
    Inventory Inventory { get; set; }
}