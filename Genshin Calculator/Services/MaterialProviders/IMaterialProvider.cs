using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.MaterialProviders;

public interface IMaterialProvider
{
    MaterialTypes SupportedType { get; }

    string GetMaterial(Character character, MaterialRarity rarity);

    IEnumerable<string> GetMaterialGroup(Character character);
}