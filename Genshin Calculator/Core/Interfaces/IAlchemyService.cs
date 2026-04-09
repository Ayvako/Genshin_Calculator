using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface IAlchemyService
{
    bool IsCraftable(MaterialTypes type);

    int ProcessCrafting(Inventory inventory, Character character, MaterialTypes type, MaterialRarity targetRarity, int amountNeeded, List<Material> alchemyTracker);
}