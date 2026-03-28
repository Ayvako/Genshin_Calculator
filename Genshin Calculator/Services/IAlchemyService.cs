using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using System.Collections.Generic;

namespace Genshin_Calculator.Services;

public interface IAlchemyService
{
    bool IsCraftable(MaterialTypes type);

    int ProcessCrafting(Inventory inventory, Character character, MaterialTypes type, MaterialRarity targetRarity, int amountNeeded, List<Material> alchemyTracker);
}