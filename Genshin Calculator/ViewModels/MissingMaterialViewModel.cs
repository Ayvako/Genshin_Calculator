using System.Windows.Media;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public class MissingMaterialViewModel
{
    public MissingMaterialViewModel(Material m, Character character)
    {
        this.Name = m.Name;
        this.Amount = m.Amount;
        this.Rarity = m.Rarity;
        this.Character = character;
    }

    public Character Character { get; set; }

    public string Name { get; }

    public int Amount { get; }

    public MaterialRarity Rarity { get; }

    public ImageSource Image => ImageService.GetMaterialImage(this.Name);
}