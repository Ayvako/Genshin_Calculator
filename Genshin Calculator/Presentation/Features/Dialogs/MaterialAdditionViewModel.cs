using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class MaterialAdditionViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalResult))]
    private int additionAmount;

    public MaterialAdditionViewModel(Material material)
    {
        this.Material = material;
        this.Material.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(this.Material.Amount))
            {
                this.OnPropertyChanged(nameof(this.TotalResult));
            }
        };
    }

    public Material Material { get; }

    public int TotalResult => this.Material.Amount + this.AdditionAmount;
}