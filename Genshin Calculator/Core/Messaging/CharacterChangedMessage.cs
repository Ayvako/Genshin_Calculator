using CommunityToolkit.Mvvm.Messaging.Messages;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Core.Messaging;

public class CharacterChangedMessage(Character character) : ValueChangedMessage<Character>(character)
{
}