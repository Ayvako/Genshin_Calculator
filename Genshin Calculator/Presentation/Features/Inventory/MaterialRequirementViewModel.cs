using Genshin_Calculator.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Presentation.Features.Inventory;

public class MaterialRequirementViewModel
{
    public MaterialRequirementViewModel(MaterialRequirement model)
    {
        this.Model = model;
        this.TargetMaterial = new MaterialViewModel(model.TargetMaterial);
        this.AlchemyCosts = [.. model.AlchemyCosts.Select(m => new MaterialViewModel(m))];
    }

    public MaterialRequirement Model { get; }

    public MaterialViewModel TargetMaterial { get; }

    public IReadOnlyList<MaterialViewModel> AlchemyCosts { get; }

    public int TotalRequired => this.Model.TotalRequired;

    public int TakenFromInventory => this.Model.TakenFromInventory;

    public int CraftedAmount => this.Model.CraftedAmount;

    public int MissingAmount => this.Model.MissingAmount;

    public bool IsCollected => this.Model.IsCollected;
}