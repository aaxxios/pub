using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PublisherBot.Handler;


[UpdateHandler]
public class NewpostMenuCallbackHandler : ICallbackQueryUpdateHandler, IResumableHandler
{
    public bool CanHandle(CallbackQuery query)
    {
        var userId = query.From.Id;
        var state = ApplicationContext.GetState(userId);
        return query.Data == "newpost" && state == UserState.StartMenu;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var userId = update.CallbackQuery!.From.Id;
        var msg = update.CallbackQuery.Message;
        ApplicationContext.SetState(userId, UserState.Text);

        // we can resume back to start menu
        await client.EditMessageTextAsync(chatId: msg!.Chat.Id, 
            messageId: msg.MessageId, "Send your post text", 
            replyMarkup: Preferences.BackMarkup);
    }

    public async Task Resume(ITelegramBotClient client, CallbackQuery query)
    {
        var userId = query!.From.Id;
        var msg = query.Message;
        ApplicationContext.SetState(userId, UserState.Text);

        // we can resume back to start menu
        await client.EditMessageTextAsync(chatId: msg!.Chat.Id,
            messageId: msg.MessageId, "Send your post text",
            replyMarkup: Preferences.BackMarkup);
    }
}

[UpdateHandler]
public class SettingsMenuCallbackHandler : ICallbackQueryUpdateHandler
{
    public bool CanHandle(CallbackQuery query)
    {
        var userId = query.From.Id;
        var state = ApplicationContext.GetState(userId);
        return query.Data == "settings" && state == UserState.StartMenu;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var userId = update.CallbackQuery!.From.Id;
        var msg = update.CallbackQuery.Message;
        ApplicationContext.SetState(userId, UserState.Settings);
        var buttons = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Language", "lang")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", "back")
            }
        };
        var repyMarkup = new InlineKeyboardMarkup(buttons);
        await client.EditMessageTextAsync(
            chatId: msg!.Chat.Id, 
            messageId: msg.MessageId, "You can configure the bot behaviour in this area", 
            replyMarkup: repyMarkup);
    }
}

[UpdateHandler]
public class BackMenuCallbackQueryHandler : ICallbackQueryUpdateHandler
{
    public bool CanHandle(CallbackQuery query)
    {
        return query.Data == "back";
    }

    public async Task Handle(ITelegramBotClient bot, Update update)
    {
        var userId = update!.CallbackQuery!.From.Id;
        var previousMenu = ApplicationContext.PopResumableFor(userId!);
        await previousMenu.Resume(bot, update.CallbackQuery);
    }
}


[UpdateHandler]
public class PostCancelCallbackHandler : ICallbackQueryUpdateHandler
{
    public bool CanHandle(CallbackQuery query)
    {
        return query.Data == "cancel";
    }

    public async Task Handle(ITelegramBotClient bot, Update update)
    {
        var userID = update.CallbackQuery!.From.Id;
        ApplicationContext.SetState(userID, UserState.Unknown);
        ApplicationContext.Initialise(userID);
        var msg = update.CallbackQuery.Message;
        await bot.EditMessageTextAsync(chatId: msg!.Chat.Id, messageId: msg.MessageId, "Operation cancelled! you can /start again");
    }
}