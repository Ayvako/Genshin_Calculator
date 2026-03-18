using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services.MaterialProviders;
using Genshin_Calculator.Services.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genshin_Calculator.Infrastructure;

public class DataIOService : IDataIOService
{
    private readonly IInventoryStore store;

    private readonly IStaticDataRepository staticData;

    private readonly IUserDataRepository userData;

    public DataIOService(IInventoryStore store, IStaticDataRepository staticData, IUserDataRepository userData)
    {
        this.store = store;
        this.staticData = staticData;
        this.userData = userData;
    }

    public void Import()
    {
        try
        {
            var characters = this.staticData.GetBaseCharacters();
            var materials = MaterialGenerator.GenerateDynamicMaterials(characters);
            materials.AddRange(this.staticData.GetStaticMaterials());

            var (userInventory, userCharacters) = this.userData.Load();

            if (userInventory != null)
            {
                MergeInventories(materials, userInventory);
            }

            if (userCharacters != null)
            {
                UpdateCharacters(characters, userCharacters);
            }

            this.store.Inventory.Characters = characters;
            this.store.Inventory.Materials = materials;
            this.store.Inventory.RefreshCache();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Import Error: {ex.Message}");
        }
    }

    public void Save()
    {
        this.userData.Save(this.store.Inventory, this.store.Inventory.Characters);
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
}