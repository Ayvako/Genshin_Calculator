using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;
using Genshin_Calculator.Infrastructure.Helpers;
using System;

namespace Genshin_Calculator.Presentation.Features.Inventory;

public partial class MaterialViewModel : ObservableObject
{
    [ObservableProperty]
    private int amount;

    public MaterialViewModel(Material model)
    {
        this.Model = model;
        this.amount = model.Amount;
    }

    public Material Model { get; }

    public string Name => this.Model.Name;

    public MaterialTypes Type => this.Model.Type;

    public MaterialRarity Rarity => this.Model.Rarity;

    public Uri ImagePath => ResourcePaths.Material(this.Model.Name);

    public void SyncToModel() => this.Model.Amount = this.Amount;

    partial void OnAmountChanged(int value)
    {
        this.Model.Amount = value;
        WeakReferenceMessenger.Default.Send(new MaterialAmountChangedMessage(this.Model));
    }
}