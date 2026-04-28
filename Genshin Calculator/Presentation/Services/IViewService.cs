using Genshin_Calculator.Models;
using System.Collections.Generic;

namespace Genshin_Calculator.Presentation.Services;

public interface IViewService
{
    void ShowCharacterEdit(Character character);

    void ShowCharacterSelector();

    void ShowInventory();

    bool ShowUpgradeCharacterDialog(Character character);

    void ShowAddMaterialsDialog(List<Material> list);
}