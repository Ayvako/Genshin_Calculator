using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Genshin_Calculator.Services.MaterialProviders;

public class MaterialProviderFactory : IMaterialProviderFactory
{
    private readonly Dictionary<MaterialTypes, IMaterialProvider> providers;

    public MaterialProviderFactory(IEnumerable<IMaterialProvider> providers)
    {
        this.providers = providers.ToDictionary(p => p.SupportedType);
    }

    public IMaterialProvider? GetProvider(MaterialTypes materialType) =>
        this.providers.GetValueOrDefault(materialType);
}