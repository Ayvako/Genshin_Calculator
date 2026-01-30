using CommunityToolkit.Mvvm.Messaging.Messages;
using Genshin_Calculator.Models;

namespace Genshin_Calculator.Messages;

// Сообщение о том, что данные персонажа изменились
public class CharacterChangedMessage(Character character) : ValueChangedMessage<Character>(character)
{
}