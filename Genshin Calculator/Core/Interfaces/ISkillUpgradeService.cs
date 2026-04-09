using System.Collections.Generic;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface ISkillUpgradeService
{
    List<Material> GetSkillsCost(Character character);
}