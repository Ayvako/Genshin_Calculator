using System.Collections.Generic;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services;

public interface ICharacterUpgradeService
{
    List<Material> GetCharacterCost(Character character);
}