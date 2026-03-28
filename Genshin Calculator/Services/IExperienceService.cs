using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services;

public interface IExperienceService
{
    long CalculateTotalExp(Inventory inventory);

    void ProcessExpRequirement(Material req, Inventory inventory, ref long totalExpPool, MaterialRequirementUI uiMat);
}