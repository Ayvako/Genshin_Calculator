using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Materials;

public interface IMaterialProvider
{
    string GetMaterial(Character character, MaterialRarity rarity);
}