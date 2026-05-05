using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using System.Collections.Generic;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public interface IMaterialProvider
{
    MaterialTypes SupportedType { get; }

    string GetMaterial(Character character, MaterialRarity rarity);

    IEnumerable<string> GetMaterialGroup(Character character);
}