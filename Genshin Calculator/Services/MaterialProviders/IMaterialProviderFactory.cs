using Genshin_Calculator.Models.Enums;

namespace Genshin_Calculator.Services.MaterialProviders;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}