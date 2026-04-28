using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Genshin_Calculator.Infrastructure.Repositories;

public class EmbeddedResourceRepository : IEmbeddedDataRepository
{
    private readonly Lazy<LevelData> levelData = new(() => Load<LevelData>("LevelCosts.json"));

    private readonly Lazy<SkillLevelData> skillData = new(() => Load<SkillLevelData>("SkillCosts.json"));

    public LevelData GetLevelCosts() => this.levelData.Value;

    public SkillLevelData GetSkillCosts() => this.skillData.Value;

    private static T Load<T>(string fileName)
    {
        var resourceName = $"Genshin_Calculator.Resources.Json.{fileName}";
        using var stream = typeof(EmbeddedResourceRepository).Assembly
                               .GetManifestResourceStream(resourceName)
                           ?? throw new FileNotFoundException(resourceName);

        using var reader = new StreamReader(stream);
        return JsonConvert.DeserializeObject<T>(reader.ReadToEnd())
               ?? throw new InvalidOperationException($"Failed to parse: {fileName}");
    }
}