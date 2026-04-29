using System;
using System.Threading.Tasks;

namespace Genshin_Calculator.Core.Interfaces;

public interface IDataIOService
{
    Task ImportAsync(IProgress<(string Message, double Percent)>? progress = null);

    void Save();
}