using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface IGameDataRepository
{
    List<Character> GetBaseCharacters();

    List<Material> GetStaticMaterials();
}