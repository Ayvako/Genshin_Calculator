using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Services.Materials;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}