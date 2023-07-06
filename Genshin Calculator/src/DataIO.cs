using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Windows.Resources;
using System.Windows;

namespace Genshin.src
{
    static class DataIO
    {

        public static void Export()
        {


            var exportInventory = Inventory.CopyDictionary(Inventory.MyInventory);

            var materialsJson = JsonConvert.SerializeObject(exportInventory, Formatting.Indented);
            var charactersJson = JsonConvert.SerializeObject(Inventory.Characters, Formatting.Indented);

            var exportJson = new JObject
            {
                ["Materials"] = JToken.Parse(materialsJson),
                ["Characters"] = JToken.Parse(charactersJson)
            };

            var exportString = exportJson.ToString(Formatting.Indented);
            File.WriteAllText("Data/Export.json", exportString);

            Console.WriteLine("Export");
        }
        public static void Import()
        {
            Uri resourceUri = new ("pack://application:,,,/Genshin Calculator;component/Resources/Json/Initializations.json");
            StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri);



            if (resourceInfo != null)
            {
                using StreamReader reader = new(resourceInfo.Stream);
                string jsonContent = reader.ReadToEnd();
                var initJson = JObject.Parse(jsonContent);

                var materials = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(initJson["Materials"].ToString());

                var merged = MergList(materials["LocalSpecialty"], materials["BookType"], materials["Gem"],
                    materials["Enemy"], materials["MiniBoss"], materials["WeeklyBoss"], materials["Other"]);

                merged.ForEach(m => Inventory.MyInventory[m] = 0);

                List<Assets> assets = JsonConvert.DeserializeObject<List<Assets>>((initJson["Characters"].ToString()));

                foreach (var asset in assets)
                {
                    Inventory.Characters.Add(new Character(asset.Name, asset));
                }
            }
            else
            {
                throw new Exception("Resource Initializations.json not found");
            }

            if (File.Exists("Data/Export.json"))
            {
                JObject updateJson = JObject.Parse(File.ReadAllText("Data/Export.json"));
                if (updateJson["Materials"] != null)
                {

                    var updateInventory = JsonConvert.DeserializeObject<Dictionary<string, int>>(updateJson["Materials"].ToString());
                    var update_characters = JsonConvert.DeserializeObject<List<Character>>(updateJson["Characters"].ToString());
                    Inventory.MyInventory = MergDictionaries(updateInventory, Inventory.MyInventory);

                    foreach (var character in Inventory.Characters)
                    {

                        var updateCharacter = update_characters.FirstOrDefault(c => c.Name == character.Name);
                        if (updateCharacter == null) continue;
                        character.Priority = updateCharacter.Priority;
                        character.CurrentLevel = updateCharacter.CurrentLevel;
                        character.DesiredLevel = updateCharacter.DesiredLevel;
                        character.AutoAttack = updateCharacter.AutoAttack;
                        character.Elemental = updateCharacter.Elemental;
                        character.Burst = updateCharacter.Burst;
                        character.Deleted = updateCharacter.Deleted;
                        character.Activated = updateCharacter.Activated;
                    }

                }
            }

            Inventory.InventoryCopy = Inventory.CopyDictionary(Inventory.MyInventory);

            Console.WriteLine("Import");
        }

        private static Dictionary<string, int> MergDictionaries(params Dictionary<string, int>[] dictionaries)
        {
            IEnumerable<KeyValuePair<string, int>> merged = dictionaries[0];
            for (int i = 1; i < dictionaries.Length; i++)
                merged = merged.Concat(dictionaries[i]);
            Dictionary<string, int> result = merged
                .ToLookup(kvp => kvp.Key, kvp => kvp.Value)
                .ToDictionary(group => group.Key, group => group.Sum(x => x));
            return result;
        }

        private static List<string> MergList(params List<string>[] lists)
        {
            List<string> result = new();

            foreach (var list in lists)
            {
                result.AddRange(list);
            }

            return result; 
        }

        public static Dictionary<string, string[]> GetMaterials(string materials)
        {
            Uri resourceUri = new($"pack://application:,,,/Genshin Calculator;component/Resources/Json/{materials}.json");
            StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri);
            using StreamReader reader = new(resourceInfo.Stream);
            string jsonContent = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<Dictionary<string, string[]>>(jsonContent);

        }

    }
}
