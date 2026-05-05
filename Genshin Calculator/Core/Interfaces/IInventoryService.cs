using Genshin_Calculator.Core.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface IInventoryService
{
    IReadOnlyList<Character> GetCharacters();

    Inventory GetInventory();

    void Upgrade(Character character);

    Dictionary<Character, List<MaterialRequirement>> CalculateMissingMaterials(Inventory sourceInventory);

    List<Material> GetRelatedMaterials(Character character, Material material);

    List<Material> TotalCost(Character character);
}