using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public partial class CharacterSelectorViewModel : ObservableObject
{
    private readonly CharacterService characterService;

    [ObservableProperty]
    private string? searchQuery;

    [ObservableProperty]
    private ObservableCollection<string> selectedElements = [];

    [ObservableProperty]
    private ObservableCollection<string> selectedWeapons = [];

    [ObservableProperty]
    private bool isSortByRarity = false;

    public CharacterSelectorViewModel(ObservableCollection<Character> availableCharacters, CharacterService characterServise)
    {
        this.characterService = characterServise;
        this.AvailableCharacters = availableCharacters;
        this.FilteredCharacters = [];

        this.ApplyFilter();
    }

    public event EventHandler<bool>? CloseRequested;

    public ObservableCollection<Character> AvailableCharacters { get; }

    public ObservableCollection<Character> FilteredCharacters { get; }

    public ObservableCollection<string> ElementTypes { get; } =
    [
        "Pyro", "Hydro", "Cryo", "Electro", "Dendro", "Geo", "Anemo"
    ];

    public ObservableCollection<string> WeaponTypes { get; } =
    [
        "Sword", "Claymore", "Polearm", "Bow", "Catalyst"
    ];

    partial void OnIsSortByRarityChanged(bool value) => ApplyFilter();

    partial void OnSearchQueryChanged(string? value) => ApplyFilter();

    partial void OnSelectedElementsChanged(ObservableCollection<string> value) => ApplyFilter();

    partial void OnSelectedWeaponsChanged(ObservableCollection<string> value) => ApplyFilter();

    private void ApplyFilter()
    {
        IEnumerable<Character> query = this.AvailableCharacters;

        // Поиск
        if (!string.IsNullOrWhiteSpace(this.SearchQuery))
        {
            query = query.Where(c =>
                c.Name.Contains(this.SearchQuery, StringComparison.OrdinalIgnoreCase));
        }

        // Элемент
        if (this.SelectedElements.Count > 0)
        {
            query = query.Where(c =>
                this.SelectedElements.Contains(c.Assets!.Element.ToString()));
        }

        // Оружие
        if (this.SelectedWeapons.Count > 0)
        {
            query = query.Where(c =>
                this.SelectedWeapons.Contains(c.Assets!.Weapon.ToString()));
        }

        // Сортировка
        query = this.IsSortByRarity
            ? query.OrderByDescending(c => c.Assets!.Rarity).ThenBy(c => c.Name)
            : query.OrderBy(c => c.Name);

        this.FilteredCharacters.Clear();
        foreach (var item in query)
        {
            this.FilteredCharacters.Add(item);
        }
    }

    [RelayCommand]
    private void ToggleWeapon(string e)
    {
        if (!this.SelectedWeapons.Remove(e))
        {
            this.SelectedWeapons.Add(e);
        }

        this.ApplyFilter();
    }

    [RelayCommand]
    private void ToggleElement(string e)
    {
        if (!this.SelectedElements.Remove(e))
        {
            this.SelectedElements.Add(e);
        }

        this.ApplyFilter();
    }

    [RelayCommand]
    private void SelectCharacter(Character character)
    {
        this.characterService.AddCharacter(character);
        this.AvailableCharacters.Remove(character);
        this.CloseRequested?.Invoke(this, true);
    }

    [RelayCommand]
    private void Cancel()
    {
        this.CloseRequested?.Invoke(this, false);
    }
}