using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Application.Services.MaterialProviders;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}