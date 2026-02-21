using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Services.Interfaces;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}