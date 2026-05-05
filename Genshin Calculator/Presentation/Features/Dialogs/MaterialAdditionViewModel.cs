using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Presentation.Features.Inventory;

namespace Genshin_Calculator.Presentation.ViewModels;

public partial class MaterialAdditionViewModel : ObservableObject, IRecipient<MaterialAmountChangedMessage>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalResult))]
    private int additionAmount;

    public MaterialAdditionViewModel(MaterialViewModel material)
    {
        this.Material = material;
        WeakReferenceMessenger.Default.Register(this);
    }

    public MaterialViewModel Material { get; }

    public int TotalResult => this.Material.Amount + this.AdditionAmount;

    public void Receive(MaterialAmountChangedMessage message)
    {
        if (message.Value == this.Material.Model)
        {
            this.OnPropertyChanged(nameof(this.TotalResult));
        }
    }
}