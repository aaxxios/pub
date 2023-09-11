using Telegram.Bot.Types;
using Telegram.Bot;

namespace PublisherBot.Handler;

/// <summary>
/// interface for <see cref="IHandler"/> that can process <see cref="Update.CallbackQuery"/> updates
/// </summary>
public interface ICallbackQueryUpdateHandler
{
    public bool CanHandle(CallbackQuery query);

    public Task Handle(ITelegramBotClient bot, Update update);
}