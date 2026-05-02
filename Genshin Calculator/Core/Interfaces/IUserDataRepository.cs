using Genshin_Calculator.Core.Models;

namespace Genshin_Calculator.Core.Interfaces;

public interface IUserDataRepository
{
    bool FileExists { get; }

    void Save(Inventory inventory);

    Inventory? Load();
}