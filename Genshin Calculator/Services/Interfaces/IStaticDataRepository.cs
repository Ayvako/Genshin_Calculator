using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Services.Interfaces;

public interface IStaticDataRepository
{
    List<Character> GetBaseCharacters();

    List<Material> GetStaticMaterials();
}