using Genshin_Calculator.Models;

namespace Genshin_Calculator.Services.Interfaces;

public interface IDialogService
{
    void ShowCharacterEdit(Character character);

    void ShowCharacterSelector();

    void ShowInventory();

    void ShowPriority();
}