using Telegram.Bot.Types;
using Telegram.Bot;

namespace PublisherBot.Handler;

/// <summary>
/// The base interface that can handle arbitary <see cref="Update"/> updates
/// </summary>
public interface IHandler
{
    public bool CanHandle(Update update);

    public Task Handle(ITelegramBotClient bot, Update update);

}
