using Genshin_Calculator.Application.Internal;
using Genshin_Calculator.Application.Services.MaterialProviders;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Genshin_Calculator.Infrastructure;

internal class DataIOService : IDataIOService
{
    private readonly DataUpdateService updater;

    private readonly InventoryStore store;

    private readonly IDataRepository data;

    private readonly IUserDataRepository userData;

    private bool isSuccessfullyLoaded = false;

    public DataIOService(InventoryStore store, IDataRepository staticData, IUserDataRepository userData, DataUpdateService updater)
    {
        this.store = store;
        this.data = staticData;
        this.userData = userData;
        this.updater = updater;
    }

    public async Task ImportAsync(IProgress<(string Message, double Percent)>? progress = null)
    {
        progress?.Report(("Checking for updates...", 5));
        await Task.Delay(200);

        await this.updater.UpdateAllDataAsync(progress);

        progress?.Report(("Loading game data...", 96));
        await Task.Delay(200);

        try
        {
            var characters = this.data.GetBaseCharacters();
            var materials = MaterialGenerator.GenerateDynamicMaterials(characters);
            var staticMaterials = this.data.GetStaticMaterials();
            materials.AddRange(staticMaterials);

            if (!this.userData.FileExists)
            {
                progress?.Report(("Starting fresh...", 98));
                await Task.Delay(200);

                this.ApplyData(characters, materials);
                this.isSuccessfullyLoaded = true;

                progress?.Report(("Done!", 100));
                await Task.Delay(200);

                return;
            }

            progress?.Report(("Merging save data...", 98));
            await Task.Delay(200);

            var inventory = this.userData.Load();

            if (inventory == null)
            {
                this.isSuccessfullyLoaded = false;
            }
            else
            {
                if (inventory != null)
                {
                    MergeInventories(materials, inventory);
                }

                if (inventory!.Characters != null)
                {
                    UpdateCharacters(characters, inventory.Characters);
                }

                this.ApplyData(characters, materials);
                this.isSuccessfullyLoaded = true;
                progress?.Report(("Done!", 100));
                await Task.Delay(200);
            }
        }
        catch (Exception ex)
        {
            this.isSuccessfullyLoaded = false;
            Debug.WriteLine($"❌ Import Error: {ex.Message}");
        }
    }

    public void Save()
    {
        if (!this.isSuccessfullyLoaded)
        {
            Debug.WriteLine("⚠️ Save blocked: Data was not loaded successfully. Avoiding data loss.");
            return;
        }

        this.userData.Save(this.store.Inventory);
    }

    private static void UpdateCharacters(List<Character> baseChars, List<Character> importedChars)
    {
        var importMap = importedChars.ToDictionary(c => c.Name);

        foreach (var c in baseChars)
        {
            if (importMap.TryGetValue(c.Name, out var update))
            {
                c.ApplyChangesFrom(update);
            }
        }
    }

    private static void MergeInventories(List<Material> baseMaterials, Inventory imported)
    {
        var importedDict = imported.Materials.ToDictionary(m => m.Name);

        foreach (var baseMat in baseMaterials)
        {
            if (importedDict.TryGetValue(baseMat.Name, out var importedMat))
            {
                baseMat.Amount = importedMat.Amount;
            }
        }
    }

    private void ApplyData(List<Character> characters, List<Material> materials)
    {
        this.store.Inventory.Characters = characters;
        this.store.Inventory.Materials = materials;
        this.store.Inventory.RefreshCache();
    }
}