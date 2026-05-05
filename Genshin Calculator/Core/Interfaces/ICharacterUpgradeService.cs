using Genshin_Calculator.Core.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface ICharacterUpgradeService
{
    List<Material> GetCharacterCost(Character character);
}