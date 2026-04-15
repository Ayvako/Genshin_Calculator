using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface IUserDataRepository
{
    bool FileExists { get; }

    void Save(Inventory inventory);

    (Inventory? Inventory, List<Character>? Characters) Load();
}