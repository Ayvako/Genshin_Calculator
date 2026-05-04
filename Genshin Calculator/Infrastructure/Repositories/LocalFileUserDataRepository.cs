using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Genshin_Calculator.Infrastructure.Repositories;

public class LocalFileUserDataRepository : IUserDataRepository
{
    private readonly string exportFilePath;

    public LocalFileUserDataRepository(IConfiguration config)
    {
        this.exportFilePath = config["Paths:ExportFile"] ?? "Data/Export.json";
    }

    public bool FileExists => File.Exists(this.exportFilePath) || File.Exists(this.exportFilePath + ".bak");

    public async Task SaveAsync(Inventory inventory)
    {
        var directory = Path.GetDirectoryName(this.exportFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string temp = this.exportFilePath + ".tmp";
        string backup = this.exportFilePath + ".bak";

        var json = Serialize(inventory);

        using (var fs = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        using (var sw = new StreamWriter(fs, Encoding.UTF8))
        {
            await sw.WriteAsync(json);
            await sw.FlushAsync();
            await fs.FlushAsync();
        }

        if (File.Exists(this.exportFilePath))
        {
            File.Replace(temp, this.exportFilePath, backup);
        }
        else
        {
            File.Move(temp, this.exportFilePath);
        }
    }

    public Inventory? Load()
    {
        var result = TryLoad(this.exportFilePath);
        if (result != null)
        {
            return result;
        }

        var backup = this.exportFilePath + ".bak";
        return File.Exists(backup) ? TryLoad(backup) : null;
    }

    private static Inventory? TryLoad(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return Deserialize(File.ReadAllText(path));
        }
        catch
        {
            return null;
        }
    }

    private static string Serialize(Inventory inventory)
    => JsonConvert.SerializeObject(inventory, Formatting.Indented);

    private static Inventory? Deserialize(string json)
        => JsonConvert.DeserializeObject<Inventory>(json);
}