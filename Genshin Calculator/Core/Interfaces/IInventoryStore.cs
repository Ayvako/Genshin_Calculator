using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface IInventoryStore
{
    Inventory Inventory { get; set; }
}