using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Interfaces;

public interface IInventoryStore
{
    Inventory Inventory { get; set; }
}