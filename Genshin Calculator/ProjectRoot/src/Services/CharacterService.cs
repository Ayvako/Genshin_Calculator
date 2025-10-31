using Genshin_Calculator.ProjectRoot.Src.Models;

namespace Genshin_Calculator.ProjectRoot.Src.Services;

public class CharacterService
{
    public static void ChangePriority(Character c1, Character c2) => (c2.Priority, c1.Priority) = (c1.Priority, c2.Priority);

    public static void EnableCharacter(Character character) => character.Activated = true;

    public static void DisableCharacter(Character character) => character.Activated = false;

    public static void DeleteCharacter(Character character) => character.Deleted = true;
}