using Genshin_Calculator.Models.Enums;

namespace Genshin_Calculator.Services.Interfaces;

public interface IMaterialProviderFactory
{
    IMaterialProvider? GetProvider(MaterialTypes materialType);
}