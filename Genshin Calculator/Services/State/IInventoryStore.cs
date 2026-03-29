using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Services.State;

public interface IInventoryStore
{
    Inventory Inventory { get; set; }
}