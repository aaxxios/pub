using Telegram.Bot.Types;
using Telegram.Bot;

namespace PublisherBot.Handler;

/// <summary>
/// interface for <see cref="IHandler"/> that can process <see cref="Update.Message"/>
/// </summary>
internal interface IMessageUpdateHandler
{
    public bool CanHandle(Message message);
    public Task Handle(ITelegramBotClient telegramBotClient, Update update);
}
