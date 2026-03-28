using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Services;

public interface IInventoryService
{
    IReadOnlyList<Character> GetCharacters();

    Inventory GetInventory();

    void Upgrade(Character character);

    Dictionary<Character, List<MaterialRequirementUI>> CalculateMissingMaterials(Inventory sourceInventory);

    List<Material> GetRelatedMaterials(Character character, Material material);
}