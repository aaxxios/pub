using Telegram.Bot;
using Telegram.Bot.Types;

namespace PublisherBot.Handler;

/// <summary>
/// interface for handlers that can resume from back button
/// </summary>
public interface IResumableHandler
{
    public Task Resume(ITelegramBotClient client, CallbackQuery query);
}
