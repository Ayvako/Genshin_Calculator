using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Core.Interfaces;

public interface IDialogService
{
    void ShowCharacterEdit(Character character);

    void ShowCharacterSelector();

    void ShowInventory();

    void ShowPriority();

    void ShowAddMaterialsDialog(List<Material> list);
}