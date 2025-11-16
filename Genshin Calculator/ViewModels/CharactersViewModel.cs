using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public class CharactersViewModel : ObservableObject
{
    public CharactersViewModel(InventoryService inventoryService)
    {
        var characterModels = inventoryService.GetCharacters();

        this.Characters = new ObservableCollection<CharacterCardViewModel>(
            characterModels.Select(model =>
                new CharacterCardViewModel(model)));
    }

    public ObservableCollection<CharacterCardViewModel> Characters { get; }
}