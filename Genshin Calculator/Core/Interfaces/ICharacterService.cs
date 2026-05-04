using Genshin_Calculator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genshin_Calculator.Core.Interfaces;

public interface ICharacterService
{
    Task UpdateCharacterAsync(Character character);

    Task ToggleCharacterActivityAsync(Character character);

    Task DeleteCharacterAsync(Character character);

    Task AddCharacterAsync(Character character);

    IReadOnlyList<Character> GetCharacters();
}