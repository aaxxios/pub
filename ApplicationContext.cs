using PublisherBot.Handler;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PublisherBot;


public class ApplicationContext
{
    private static Dictionary<long, UserState> State = new();
    public static Dictionary<long, Post> Data = new();

    public static Dictionary<long, Stack<IResumableHandler>> Resumables = new();

    public static List<Post> Posts = new();

    public static UserState GetState(long userId)
    {
        if(State.TryGetValue(userId, out var state))
        {
            return state;
        }
        return UserState.Unknown;
    }
    
    public static void SetState(long userId, UserState state)
    {
        State[userId] = state;
        
    }

    public static void Initialise(long userId)
    {
        Data[userId] = new Post();
        Resumables[userId] = new Stack<IResumableHandler>();
    }

    public static void PushResumableFor(long userId, IResumableHandler resumable)
    {
        Resumables[userId].Push(resumable);
    }

    public static IResumableHandler PopResumableFor(long usserId)
    {
        if(Resumables.TryGetValue(usserId, out var resumables))
        {
            return resumables.Pop();
        }
        return new InactiveResumable();
    }
}


public class InactiveResumable : IResumableHandler
{
    public Task Resume(ITelegramBotClient client, CallbackQuery query)
    {
        Console.WriteLine("Returened inactive resumable");
        return Task.CompletedTask;
    }
}


public enum UserState
{
    Chat, Media, Button, Text, Unknown, Duration, StartMenu, Settings
}

#pragma warning disable CS8618
public class Post
{
    public string PostText { get; set; }
    public string Media { get; set; }
    public short MediaType { get; set; }

    public InlineKeyboardMarkup Markup { get; set; }

    public int PostInterval { get; set; }

    public DateTime NextPost { get; set; }
    public long Chat { get; set; }
}

#pragma warning restore CS8618