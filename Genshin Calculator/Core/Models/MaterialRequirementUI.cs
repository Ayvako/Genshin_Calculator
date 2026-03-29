using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Models;

public class MaterialRequirementUI
{
    public MaterialRequirementUI(Material targetMaterial, int totalRequired)
    {
        TargetMaterial = targetMaterial;
        TotalRequired = totalRequired;
    }

    public Material TargetMaterial { get; set; }

    public int TotalRequired { get; set; }

    public int TakenFromInventory { get; set; }

    public int CraftedAmount { get; set; }

    public List<Material> AlchemyCosts { get; set; } = [];

    public int MissingAmount { get; set; }

    public bool IsCollected => MissingAmount <= 0;
}