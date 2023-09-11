using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PublisherBot;

internal class Preferences
{
    public static UpdateType[] AllowedUpdates =
        {
            UpdateType.Message, UpdateType.CallbackQuery
        };

    public static InlineKeyboardMarkup CancelMarkup = new(new InlineKeyboardButton("Cancel")
    {
        CallbackData = "cancel"
    });

    public static InlineKeyboardMarkup BackMarkup = new(new InlineKeyboardButton("Back")
    {
        CallbackData = "back"
    });

}