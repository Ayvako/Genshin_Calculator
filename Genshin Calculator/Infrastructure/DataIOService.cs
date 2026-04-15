using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.MaterialProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genshin_Calculator.Infrastructure;

public class DataIOService : IDataIOService
{
    private readonly IInventoryStore store;

    private readonly IDataRepository data;

    private readonly IUserDataRepository userData;

    private bool isSuccessfullyLoaded = false;

    public DataIOService(IInventoryStore store, IDataRepository staticData, IUserDataRepository userData)
    {
        this.store = store;
        this.data = staticData;
        this.userData = userData;
    }

    public void Import()
    {
        try
        {
            var characters = this.data.GetBaseCharacters();
            var materials = MaterialGenerator.GenerateDynamicMaterials(characters);
            materials.AddRange(this.data.GetStaticMaterials());

            if (!this.userData.FileExists)
            {
                Debug.WriteLine("ℹ️ No save file found. Starting fresh.");
                this.ApplyData(characters, materials);
                this.isSuccessfullyLoaded = true;
                return;
            }

            var (userInventory, userCharacters) = this.userData.Load();

            if (userInventory == null && userCharacters == null)
            {
                this.isSuccessfullyLoaded = false;
                Debug.WriteLine("❌ CRITICAL: Save file exists but is corrupted. Saving disabled to prevent data loss.");
            }
            else
            {
                if (userInventory != null)
                {
                    MergeInventories(materials, userInventory);
                }

                if (userCharacters != null)
                {
                    UpdateCharacters(characters, userCharacters);
                }

                this.ApplyData(characters, materials);
                this.isSuccessfullyLoaded = true;
                Debug.WriteLine("✅ Data loaded and merged successfully.");
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