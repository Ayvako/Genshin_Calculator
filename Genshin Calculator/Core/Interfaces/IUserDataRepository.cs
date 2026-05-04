using Genshin_Calculator.Core.Models;
using System.Threading.Tasks;

namespace Genshin_Calculator.Core.Interfaces;

public interface IUserDataRepository
{
    bool FileExists { get; }

    Task SaveAsync(Inventory inventory);

    Inventory? Load();
}