using System.Collections.Generic;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Interfaces;

public interface ICharacterUpgradeService
{
    List<Material> GetCharacterCost(Character character);
}