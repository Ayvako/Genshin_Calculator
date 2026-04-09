using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface IExperienceService
{
    long CalculateTotalExp(Inventory inventory);

    void ProcessExpRequirement(Material req, Inventory inventory, ref long totalExpPool, MaterialRequirement uiMat);

    Material ConvertXpToHeroWit(long totalXpAmount);
}