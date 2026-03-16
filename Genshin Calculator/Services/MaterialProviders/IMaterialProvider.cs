using Genshin_Calculator.Models;
using Genshin_Calculator.Models.Enums;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.MaterialProviders;

public interface IMaterialProvider
{
    string GetMaterial(Character character, MaterialRarity rarity);

    IEnumerable<string> GetMaterialGroup(Character character);
}