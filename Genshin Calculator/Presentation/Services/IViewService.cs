using Genshin_Calculator.Core.Models;
using Genshin_Calculator.Presentation.Features.Characters;
using System.Collections.Generic;

namespace Genshin_Calculator.Presentation.Services;

public interface IViewService
{
    void ShowCharacterEdit(CharacterViewModel character);

    void ShowCharacterSelector();

    void ShowInventory();

    bool ShowUpgradeCharacterDialog(CharacterViewModel character);

    void ShowAddMaterialsDialog(List<Material> list);

    bool ShowConfirm(string title, string message);
}