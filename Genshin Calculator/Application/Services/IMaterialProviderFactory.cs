using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Core.Interfaces;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}