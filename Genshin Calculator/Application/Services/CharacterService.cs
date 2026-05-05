using CommunityToolkit.Mvvm.Messaging;
using Genshin_Calculator.Core.Interfaces;
using Genshin_Calculator.Core.Messaging;
using Genshin_Calculator.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Genshin_Calculator.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IInventoryService inventoryService;

    private readonly SemaphoreSlim semaphore = new(1, 1);

    public CharacterService(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
    }

    public async Task UpdateCharacterAsync(Character character)
    {
        WeakReferenceMessenger.Default.Send(new CharacterChangedMessage(character));
        await Task.CompletedTask;
    }

    public async Task ToggleCharacterActivityAsync(Character character)
    {
        character.Activated = !character.Activated;
        await this.UpdateCharacterAsync(character);
    }

    public async Task DeleteCharacterAsync(Character character)
    {
        character.Deleted = true;
        character.Reset();
        await this.UpdateCharacterAsync(character);
    }

    public async Task AddCharacterAsync(Character character)
    {
        await this.semaphore.WaitAsync();
        try
        {
            character.Deleted = false;
            character.Activated = true;

            await Task.Run(async () =>
            {
                var allCharacters = this.GetCharacters().ToList();
                int maxPriority = allCharacters.Count > 0
                    ? allCharacters.Max(c => c.Priority)
                    : 0;

                character.Priority = maxPriority + 1;
                await this.UpdateCharacterAsync(character);
            });
        }
        finally
        {
            this.semaphore.Release();
        }
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return this.inventoryService.GetCharacters();
    }
}