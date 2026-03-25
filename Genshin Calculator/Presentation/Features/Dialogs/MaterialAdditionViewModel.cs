using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Models;
using System.ComponentModel;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class MaterialAdditionViewModel : ObservableObject, IRecipient<MaterialAmountChangedMessage>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalResult))]
    private int additionAmount;

    public MaterialAdditionViewModel(Material material)
    {
        this.Material = material;
        WeakReferenceMessenger.Default.Register(this);
    }

    public Material Material { get; }

    public int TotalResult => this.Material.Amount + this.AdditionAmount;

    public void Receive(MaterialAmountChangedMessage message)
    {
        if (message.Value == this.Material)
        {
            this.OnPropertyChanged(nameof(this.TotalResult));
        }
    }

    private void OnMaterialPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(this.Material.Amount))
        {
            this.OnPropertyChanged(nameof(this.TotalResult));
        }
    }
}