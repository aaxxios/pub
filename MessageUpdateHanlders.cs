using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using PublisherBot.Data;

namespace PublisherBot.Handler;


[UpdateHandler]
internal class StartHanlder : IMessageUpdateHandler, IResumableHandler
{

    public bool CanHandle(Message message)
    {
        // message cannot be null here
        return message.Text is string msg && msg.StartsWith("/start") &&
             Configuration.Configuration.Instance.permittedusers.Contains(message.From!.Id) &&
             message.Chat.Type is ChatType.Private;
    }
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        if (update.Message is Message msg && msg.Text is string)
        {
            //using(var context = new ApplicationDbContext())
            //{
            //    var user = context.TelegramUsers.FirstOrDefault(u => u.Id == msg.From!.Id);
            //    if (user != null)
            //    {
            //        Console.WriteLine("User not found");
            //    }
            //}
            var buttons = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("New Post", "newpost"),
                //InlineKeyboardButton.WithCallbackData("Settings", "settings")
            };
            
            var replyMarkup = new InlineKeyboardMarkup(buttons);
            var userID = msg.From!.Id;
            ApplicationContext.SetState(userID, UserState.StartMenu);
            ApplicationContext.Initialise(userID); // initialise the data associated with the user
            ApplicationContext.PushResumableFor(userID, this as IResumableHandler);
            await client.SendTextMessageAsync(userID, "Select action", replyMarkup: replyMarkup);
            Logging.LogInfo($"Bot started by {userID}");
        }
    }

    public async Task Resume(ITelegramBotClient client, CallbackQuery query)
    {
        var buttons = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData("New Post", "newpost"),
                //InlineKeyboardButton.WithCallbackData("Settings", "settings")
            };
        var replyMarkup = new InlineKeyboardMarkup(buttons);
        var userID = query.From!.Id;
        ApplicationContext.SetState(userID, UserState.StartMenu);
        ApplicationContext.Initialise(userID); // initialise the data associated with the user
        ApplicationContext.PushResumableFor(userID, this as IResumableHandler);
        await client.EditMessageTextAsync(
            chatId: query.Message!.Chat.Id, 
            messageId: query.Message.MessageId, text: "Select action", replyMarkup: replyMarkup);
    }
}

[UpdateHandler]
internal class PostTextHandlder : IMessageUpdateHandler
{

    public bool CanHandle(Message message)
    {
        var state = ApplicationContext.GetState(message.From!.Id);
        // message cannot be null here
        return state == UserState.Text && message.Text is string && message.Chat.Type is ChatType.Private;
    }
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        if (update.Message is Message msg && msg.Text is string text)
        {
            var userID = msg.From!.Id;
            ApplicationContext.SetState(userID, UserState.Media);
            ApplicationContext.Data[userID].PostText = text;
            ApplicationContext.PushResumableFor(userID, new NewpostMenuCallbackHandler() as IResumableHandler);
            await client.SendTextMessageAsync(userID, "Send the media for the post", replyMarkup: Preferences.BackMarkup);
        }
    }
}

[UpdateHandler]
internal class PostMediaHandler : IMessageUpdateHandler
{
    public bool CanHandle(Message message)
    {
        var state = ApplicationContext.GetState(message.From!.Id);
        // message cannot be null here
        return state == UserState.Media &&
            (message.Photo is PhotoSize[] || message.Video is Video || message.Animation is Animation) &&
            message.Chat.Type == ChatType.Private;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var userID = update.Message!.From!.Id;
        var msg = update.Message;
        var data = ApplicationContext.Data[userID];

        switch (msg.Type)
        {
            case MessageType.Photo:
                {

                    data.Media = msg.Photo!.First().FileId;
                    data.MediaType = 0;
                    ApplicationContext.SetState(userID, UserState.Chat);
                    break;
                }
            case MessageType.Video:
                {
                    data.Media = msg.Video!.FileId;
                    data.MediaType = 1;
                    ApplicationContext.SetState(userID, UserState.Chat);
                    break;
                }
            case MessageType.Animation:
                {
                    data.Media = msg.Animation!.FileId;
                    data.MediaType = 2;
                    ApplicationContext.SetState(userID, UserState.Chat);
                    break;
                }
            default:
                {
                    await client.SendTextMessageAsync(userID, "Only (Video | Picture | Gif) is supported");
                    return;
                }
        }


        var button = new KeyboardButton("Select the channel")
        {
            RequestChat = new KeyboardButtonRequestChat()
            {
                BotIsMember = true,
                ChatIsCreated = true, ChatIsChannel = true,
                BotAdministratorRights = new ChatAdministratorRights()
                {
                    CanChangeInfo = false,
                    CanDeleteMessages = true,
                    CanEditMessages = true,
                    CanInviteUsers = false,
                    CanManageChat = true,
                    CanManageTopics = false,
                    CanManageVideoChats = false,
                    CanPinMessages = true,
                    CanPostMessages = true,
                    CanPromoteMembers = false,
                    CanRestrictMembers = false,
                    IsAnonymous = false,
                }
            }
        };

        var replyMarkup = new ReplyKeyboardMarkup(button)
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };
        await client.SendTextMessageAsync(
            userID,
            "Select the channel to publish",
            replyMarkup: replyMarkup
            );
    }
}

[UpdateHandler]
public class PostChatHandler : IMessageUpdateHandler
{
    public bool CanHandle(Message message)
    {
        var state = ApplicationContext.GetState(message.From!.Id);
        // message cannot be null here
        return state == UserState.Chat &&
            (message.ChatShared is ChatShared) &&
            message.Chat.Type == ChatType.Private;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var userId = update.Message!.From!.Id;
        ApplicationContext.Data[userId].Chat = update.Message.ChatShared!.ChatId;
        var replyMarkup = new ReplyKeyboardRemove();
        ApplicationContext.SetState(userId, UserState.Button);
        await client.SendTextMessageAsync(userId,
            "Send the button to attach to the post in the format\n\n" +
            "name ## link\n\n" +
            "Example\n Admin ## https://t.me/frozymelon",
            replyMarkup: replyMarkup, disableWebPagePreview: true);
    }
}

[UpdateHandler]
public class PostButtonHandler : IMessageUpdateHandler
{
    private Regex regex = new(@"\S+\W+##\W+https?://\S+");
    public bool CanHandle(Message message)
    {
        var state = ApplicationContext.GetState(message.From!.Id);
        // message cannot be null here
        return state == UserState.Button &&
            (message.Text is string text && IsMatch(text)) &&
            message.Chat.Type == ChatType.Private;
    }

    private bool IsMatch(string text)
    {
        return regex.IsMatch(text);
    }

    private string GetMatch(string text)
    {
        return regex.Match(text).Value;
    }
    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var userId = update.Message!.From!.Id;
        var text = GetMatch(update.Message!.Text!);

        var data = text.Split("##");
        var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton(data[0]) { Url = data[1].Trim() });
        ApplicationContext.Data[userId].Markup = replyMarkup;
        ApplicationContext.Data[userId].RawMarkup = update.Message.Text!.Trim();
        ApplicationContext.SetState(userId, UserState.Duration);
        await client.SendTextMessageAsync(userId, "Send the duration to publish the post in minutes", replyMarkup: Preferences.CancelMarkup);
    }

}

[UpdateHandler]
public class PostDurationHandler : IMessageUpdateHandler
{
    private Regex regex = new(@"\d+");
    public bool CanHandle(Message message)
    {
        var state = ApplicationContext.GetState(message.From!.Id);
        // message cannot be null here
        return state == UserState.Duration &&
            (message.Text is string text && IsMatch(text)) &&
            message.Chat.Type == ChatType.Private;
    }

    private bool IsMatch(string text)
    {
        return regex.IsMatch(text);
    }

    private string GetMatch(string text)
    {
        return regex.Match(text).Value;
    }

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        var userId = update.Message!.From!.Id;
        if (int.TryParse(GetMatch(update.Message!.Text!), out var duration))
        {
            var data = ApplicationContext.Data[userId];
            data.PostInterval = duration;
            
            InputFileId file = new(data.Media);
            switch (data.MediaType)
            {
                case 0:
                    {
                        await client.SendPhotoAsync(
                            data.Chat, photo: file, caption: data.PostText, replyMarkup: data.Markup);
                        break;
                    }
                case 1:
                    {
                        await client.SendVideoAsync(
                            data.Chat, video: file, caption: data.PostText, replyMarkup: data.Markup);
                        break;
                    }
                case 2:
                    {
                        await client.SendAnimationAsync(
                            data.Chat, animation: file, caption: data.PostText, replyMarkup: data.Markup);
                        break;
                    }
            }
            data.NextPost = DateTime.UtcNow + TimeSpan.FromMinutes(duration);
            var post = new PublisherBot.Models.Post()
            {
                PostInterval = duration,
                Chat = data.Chat,
                Media = data.Media,
                MediaType = data.MediaType,
                PostText = data.PostText, NextPost = data.NextPost, Markup = data.RawMarkup
            };

           
            await DatabaseProvider.Instance.AddAsync(post);
            await DatabaseProvider.Instance.SaveChangesAsync();
            ApplicationContext.Posts.Add(data);
            await client.SendTextMessageAsync(
                userId, "The post has been published");
            ApplicationContext.SetState(userId, UserState.Unknown);
        }

    }
}