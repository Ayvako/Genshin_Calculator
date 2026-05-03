using System;
using System.Threading.Tasks;

namespace Genshin_Calculator.Core.Interfaces;

public interface IDataUpdateService
{
    Task UpdateAllDataAsync(IProgress<(string Message, double Percent)>? progress = null, double fromPercent = 0, double toPercent = 100);
}