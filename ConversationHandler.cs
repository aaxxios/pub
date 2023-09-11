using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PublisherBot.Handler;


/// <summary>
/// convenient class for handling <see cref="Update"/>s as a conversation
/// </summary>
public class ConversationHandler
{
    
    private static List<IHandler> CallbackHandlers = new();
    private static List<IHandler> MessageHandlers = new();


    public static async Task Handle(ITelegramBotClient client, Update update)
    {
        if(update.Message is Message)
        {
            foreach(var handler in MessageHandlers)
            {
                if (handler.CanHandle(update))
                {
                    await handler.Handle(client, update);
                    break;
                }
            }
        }
        else if(update.CallbackQuery is CallbackQuery)
        {
            foreach(var handler in CallbackHandlers)
            {
                if (handler.CanHandle(update))
                {
                    await handler.Handle(client, update);
                    break;
                }
            }
        }
    }


    public static void AddHanlder(IHandler handler)
    {
        if(handler is MessageHanlder)
        {
            MessageHandlers.Add(handler);
        }
        else if(handler is CallbackQueryHandler)
        {
            CallbackHandlers.Add(handler);
        }
    }
}


