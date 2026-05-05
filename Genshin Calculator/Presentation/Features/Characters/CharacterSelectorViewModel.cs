using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Core.Models.Enums;

namespace Genshin_Calculator.Presentation.Features.Characters;

public partial class CharacterSelectorViewModel : ObservableObject, IDisposable
{
    private readonly ICharacterService characterService;

    private bool disposed;

    private CancellationTokenSource? filterCts;

    [ObservableProperty]
    private string? searchQuery;

    [ObservableProperty]
    private ObservableCollection<Element> selectedElements = [];

    [ObservableProperty]
    private ObservableCollection<WeaponType> selectedWeapons = [];

    [ObservableProperty]
    private ObservableCollection<MaterialRarity> selectedCharactersRarities = [];

    [ObservableProperty]
    private bool isSortByRarity = false;

    public CharacterSelectorViewModel(ICharacterService characterService)
    {
        this.characterService = characterService;
        this.AvailableCharacters = [];
        this.FilteredCharacters = [];
    }

    public event EventHandler<bool>? CloseRequested;

    public ObservableCollection<CharacterViewModel> AvailableCharacters { get; }

    public ObservableCollection<CharacterViewModel> FilteredCharacters { get; }

    public ObservableCollection<Element> ElementTypes { get; } = new(Enum.GetValues<Element>());

    public ObservableCollection<WeaponType> WeaponTypes { get; } = new(Enum.GetValues<WeaponType>());

    public ObservableCollection<MaterialRarity> CharactersRarityTypes { get; } =
    [
        MaterialRarity.Violet, MaterialRarity.Orange
    ];

    public bool IsVioletRaritySelected => this.SelectedCharactersRarities.Contains(MaterialRarity.Violet);

    public bool IsOrangeRaritySelected => this.SelectedCharactersRarities.Contains(MaterialRarity.Orange);

    [RelayCommand]
    public async Task InitializeAsync()
    {
        var deletedChars = this.characterService.GetCharacters().Where(c => c.Deleted).ToList();

        this.AvailableCharacters.Clear();
        foreach (var c in deletedChars)
        {
            this.AvailableCharacters.Add(new CharacterViewModel(c));
        }

        await this.ApplyFilterAsync();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.filterCts?.Cancel();
                this.filterCts?.Dispose();
            }

            this.disposed = true;
        }
    }

    partial void OnIsSortByRarityChanged(bool value) => _ = ApplyFilterAsync();

    partial void OnSearchQueryChanged(string? value) => _ = ApplyFilterAsync();

    partial void OnSelectedElementsChanged(ObservableCollection<Element> value) => _ = ApplyFilterAsync();

    partial void OnSelectedWeaponsChanged(ObservableCollection<WeaponType> value) => _ = ApplyFilterAsync();

    partial void OnSelectedCharactersRaritiesChanged(ObservableCollection<MaterialRarity> value)
    {
        this.NotifyRaritySelectionChanged();
        _ = ApplyFilterAsync();
    }

    private async Task ApplyFilterAsync()
    {
        this.filterCts?.CancelAsync();
        this.filterCts = new CancellationTokenSource();
        var token = this.filterCts.Token;

        var search = this.SearchQuery;
        var elements = this.SelectedElements.ToList();
        var weapons = this.SelectedWeapons.ToList();
        var rarities = this.SelectedCharactersRarities.ToList();
        var sortByRarity = this.IsSortByRarity;
        var available = this.AvailableCharacters.ToList();

        try
        {
            var filteredResult = await Task.Run(
                () =>
            {
                IEnumerable<CharacterViewModel> query = available;

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(c =>
                        c.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                if (elements.Count > 0)
                {
                    query = query.Where(c => elements.Contains(c.Assets!.Element));
                }

                if (weapons.Count > 0)
                {
                    query = query.Where(c => weapons.Contains(c.Assets!.Weapon));
                }

                if (rarities.Count > 0)
                {
                    query = query.Where(c => rarities.Contains(c.Assets!.Rarity));
                }

                query = sortByRarity
                    ? query.OrderByDescending(c => c.Assets!.Rarity).ThenBy(c => c.Name)
                    : query.OrderBy(c => c.Name);

                return query.ToList();
            },
                token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            this.FilteredCharacters.Clear();
            foreach (var item in filteredResult)
            {
                this.FilteredCharacters.Add(item);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignored - это ожидаемое исключение при отмене задачи
        }
    }

    [RelayCommand]
    private async Task ToggleWeaponAsync(WeaponType e)
    {
        if (!this.SelectedWeapons.Remove(e))
        {
            this.SelectedWeapons.Add(e);
        }

        await this.ApplyFilterAsync();
    }

    [RelayCommand]
    private async Task ToggleRarityAsync(MaterialRarity e)
    {
        if (!this.SelectedCharactersRarities.Remove(e))
        {
            this.SelectedCharactersRarities.Add(e);
        }

        this.NotifyRaritySelectionChanged();
        await this.ApplyFilterAsync();
    }

    [RelayCommand]
    private async Task ToggleElementAsync(Element e)
    {
        if (!this.SelectedElements.Remove(e))
        {
            this.SelectedElements.Add(e);
        }

        await this.ApplyFilterAsync();
    }

    [RelayCommand]
    private async Task SelectCharacterAsync(CharacterViewModel character)
    {
        await this.characterService.AddCharacterAsync(character.Model);
        this.CloseRequested?.Invoke(this, true);
    }

    [RelayCommand]
    private void Cancel()
    {
        this.CloseRequested?.Invoke(this, true);
    }

    private void NotifyRaritySelectionChanged()
    {
        this.OnPropertyChanged(nameof(this.IsVioletRaritySelected));
        this.OnPropertyChanged(nameof(this.IsOrangeRaritySelected));
    }
}