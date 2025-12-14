using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Genshin_Calculator.Helpers.Enums;
using Genshin_Calculator.Models;
using Genshin_Calculator.Services;

namespace Genshin_Calculator.ViewModels;

public partial class CharacterSelectorViewModel : ObservableObject
{
    private readonly CharacterService characterService;

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

    public ObservableCollection<Element> ElementTypes { get; } = new(Enum.GetValues<Element>());

    public ObservableCollection<WeaponType> WeaponTypes { get; } = new(Enum.GetValues<WeaponType>());

    public ObservableCollection<MaterialRarity> CharactersRarityTypes { get; } =
    [
        MaterialRarity.Violet, MaterialRarity.Orange
    ];

    partial void OnIsSortByRarityChanged(bool value) => ApplyFilter();

    partial void OnSearchQueryChanged(string? value) => ApplyFilter();

    partial void OnSelectedElementsChanged(ObservableCollection<Element> value) => ApplyFilter();

    partial void OnSelectedWeaponsChanged(ObservableCollection<WeaponType> value) => ApplyFilter();

    partial void OnSelectedCharactersRaritiesChanged(ObservableCollection<MaterialRarity> value) => ApplyFilter();

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
                this.SelectedElements.Contains(c.Assets!.Element));
        }

        // Оружие
        if (this.SelectedWeapons.Count > 0)
        {
            query = query.Where(c =>
                this.SelectedWeapons.Contains(c.Assets!.Weapon));
        }

        // Редкость
        if (this.SelectedCharactersRarities.Count > 0)
        {
            query = query.Where(c =>
                this.SelectedCharactersRarities.Contains(c.Assets!.Rarity));
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
    private void ToggleWeapon(WeaponType e)
    {
        if (!this.SelectedWeapons.Remove(e))
        {
            this.SelectedWeapons.Add(e);
        }

        this.ApplyFilter();
    }

    [RelayCommand]
    private void ToggleRarity(MaterialRarity e)
    {
        if (!this.SelectedCharactersRarities.Remove(e))
        {
            this.SelectedCharactersRarities.Add(e);
        }

        this.ApplyFilter();
    }

    [RelayCommand]
    private void ToggleElement(Element e)
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