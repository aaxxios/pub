using Telegram.Bot.Types;
using Telegram.Bot;

namespace PublisherBot.Handler;

/// <summary>
/// implements <see cref="IHandler"/> that can process <see cref="Update.CallbackQuery"/>
/// </summary>
/// 
public class CallbackQueryHandler : IHandler
{
    private ICallbackQueryUpdateHandler Handler;

    public CallbackQueryHandler(ICallbackQueryUpdateHandler handler)
    {
        Handler = handler;
    }

    /// <summary>
    /// returns true if the update has a <see cref="Update.CallbackQuery"/> and the 
    /// <see cref="ICallbackQueryUpdateHandler.CanHandle(CallbackQuery)"/> returns true for the callbackquery
    /// associated with the message. If the result is true, this <see cref="Handle(ITelegramBotClient, Update)"/>
    /// should be called for this update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public bool CanHandle(Update update)
    {
        return update.CallbackQuery is CallbackQuery query && Handler.CanHandle(query);
    }

    /// <summary>
    /// calls the <see cref="ICallbackQueryUpdateHandler.Handle(ITelegramBotClient, Update)"/> associated with this 
    /// handler to handle the callbackquery
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    public async Task Handle(ITelegramBotClient bot, Update update)
    {
        await Handler.Handle(bot, update);
    }
}


/// <summary>
/// implements <see cref="IHandler"/> that can process <see cref="Update.Message"/>
/// </summary>
internal class MessageHanlder : IHandler
{
    private IMessageUpdateHandler Handler;

    /// <summary>
    /// initialise the <see cref="MessageHanlder"/> with a <see cref="IMessageUpdateHandler"/> used
    /// by it
    /// </summary>
    /// <param name="handler"></param>
    public MessageHanlder(IMessageUpdateHandler handler)
    {
        Handler = handler;
    }

    /// <summary>
    /// return true if the update has a message and the <see cref="IMessageUpdateHandler.CanHandle(Message)"/> 
    /// return true for the message
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public bool CanHandle(Update update)
    {
        return update.Message is Message msg && Handler.CanHandle(msg);
    }

    /// <summary>
    /// calls the <see cref="IMessageUpdateHandler.Handle(ITelegramBotClient, Update)"/> associated with this 
    /// handler to handle the message
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    public async Task Handle(ITelegramBotClient bot, Update update)
    {
        await Handler.Handle(bot, update);
    }
}
