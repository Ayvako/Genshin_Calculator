using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.Interfaces;

public interface IMaterialProvider
{
    string GetMaterial(Character character, MaterialRarity rarity);

    IEnumerable<string> GetMaterialGroup(Character character);
}