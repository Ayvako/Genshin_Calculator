using System.Collections.Generic;

namespace Genshin_Calculator.Core.Models;

public class MaterialRequirement
{
    public MaterialRequirement(Material targetMaterial, int totalRequired)
    {
        this.TargetMaterial = targetMaterial;
        this.TotalRequired = totalRequired;
    }

    public Material TargetMaterial { get; set; }

    public int TotalRequired { get; set; }

    public int TakenFromInventory { get; set; }

    public int CraftedAmount { get; set; }

    public List<Material> AlchemyCosts { get; set; } = [];

    public int MissingAmount { get; set; }

    public bool IsCollected => this.MissingAmount <= 0;
}