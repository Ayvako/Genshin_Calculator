using Genshin_Calculator.Core.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface ISkillUpgradeService
{
    List<Material> GetSkillsCost(Character character);
}