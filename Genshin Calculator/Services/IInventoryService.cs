using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Services;

public interface IInventoryService
{
    IReadOnlyList<Character> GetCharacters();

    Inventory GetInventory();

    public void Upgrade(Character character, Inventory inventory);

    Dictionary<Character, List<Material>> CalculateMissingMaterials(Inventory sourceInventory);

    List<Material> GetRelatedMaterials(Character character, Material material);

    List<Material> TotalCost(Character character);
}