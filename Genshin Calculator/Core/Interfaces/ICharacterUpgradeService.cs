using System.Collections.Generic;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface ICharacterUpgradeService
{
    List<Material> GetCharacterCost(Character character);
}