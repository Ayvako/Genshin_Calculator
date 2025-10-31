using System.Collections.Generic;
using Genshin_Calculator.ProjectRoot.src.LevelingResources;
using Genshin_Calculator.ProjectRoot.Src.Models;
using Genshin_Calculator.ProjectRoot.Src.Upgrades;

namespace Genshin_Calculator.ProjectRoot.Src.Services
{
    public class CharacterUpgradeService
    {
        public List<Material> GetCharacterCost(Character character)
        {
            return CharacterUpgrade.GetCost(character, character.CurrentLevel, character.DesiredLevel);
        }
    }
}
