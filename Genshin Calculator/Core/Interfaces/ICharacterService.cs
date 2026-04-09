using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Services;

public interface ICharacterService
{
    void UpdateCharacter(Character character);

    void ToggleCharacterActivity(Character character);

    void DeleteCharacter(Character character);

    void AddCharacter(Character character);

    IReadOnlyList<Character> GetCharacters();
}