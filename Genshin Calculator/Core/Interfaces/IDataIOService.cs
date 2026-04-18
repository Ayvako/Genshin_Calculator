using System.Threading.Tasks;

namespace Genshin_Calculator.Core.Interfaces;

public interface IDataIOService
{
    Task ImportAsync();

    void Save();
}