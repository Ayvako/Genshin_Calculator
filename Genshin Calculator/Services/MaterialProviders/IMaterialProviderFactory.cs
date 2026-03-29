using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Services.MaterialProviders;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}