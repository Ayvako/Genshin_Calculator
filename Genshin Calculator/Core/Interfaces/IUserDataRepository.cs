using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface IUserDataRepository
{
    void Save(Inventory inventory, List<Character> characters);

    (Inventory? Inventory, List<Character>? Characters) Load();
}