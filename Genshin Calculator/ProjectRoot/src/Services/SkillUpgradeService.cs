using System.Collections.Generic;
using Genshin_Calculator.ProjectRoot.src.LevelingResources;
using Genshin_Calculator.ProjectRoot.Src.Models;
using Genshin_Calculator.ProjectRoot.Src.Upgrades;

namespace Genshin_Calculator.ProjectRoot.Src.Services
{
    public class SkillUpgradeService
    {
        public List<Material> GetSkillsCost(Character character)
        {
            Skill auto_attack = character.AutoAttack;
            Skill elemental = character.Elemental;
            Skill burst = character.Burst;

            List<Material> materialsForAA = SkillUpgrade.GetCost(character, auto_attack.CurrentLevel, auto_attack.DesiredLevel);
            List<Material> materialsForElem = SkillUpgrade.GetCost(character, elemental.CurrentLevel, elemental.DesiredLevel);
            List<Material> materialsForBurst = SkillUpgrade.GetCost(character, burst.CurrentLevel, burst.DesiredLevel);

            return InventoryUtils.Merge(materialsForAA, materialsForElem, materialsForBurst);
        }
    }
}