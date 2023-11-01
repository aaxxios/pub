using PublisherBot.Data;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PublisherBot;

internal class Scheduler
{
#pragma warning disable CS8618
    public static System.Timers.Timer timer;
    private static ITelegramBotClient botClient;
    private static ApplicationDbContext context;
    private static bool running = false;
    public static void Start(ITelegramBotClient client)
    {
        botClient = client;
        context = DatabaseProvider.Instance;
        timer = new System.Timers.Timer(2000);
        timer.Enabled = true;
        timer.AutoReset = true;
        timer.Elapsed += ElapsedHandler;
        timer.Start();

    }

    public static void ElapsedHandler(object? sender, ElapsedEventArgs e)
    {
        if(running)
        {
            return;
        }
        running = true;
        Task.Run(async () =>
        {
            await publish();
        }).Wait();
        running = false;
    }

    private static async Task publish()
    {
        foreach (var data in context.Posts.ToList())
        {

            try
            {
                if (DateTime.UtcNow >= data.NextPost)
                {
                    var markup = data.Markup.Split("##");
                    var kb = new InlineKeyboardMarkup(new[]
                    {
                InlineKeyboardButton.WithUrl(markup[0].Trim(), markup[1].Trim())
            });
                    InputFileId file = new(data.Media);
                    switch (data.MediaType)
                    {
                        case 0:
                            {
                                await botClient.SendPhotoAsync(
                                    data.Chat, photo: file, caption: data.PostText, replyMarkup: kb);
                                break;
                            }
                        case 1:
                            {
                                await botClient.SendVideoAsync(
                                    data.Chat, video: file, caption: data.PostText, replyMarkup: kb);
                                break;
                            }
                        case 2:
                            {
                                await botClient.SendAnimationAsync(
                                    data.Chat, animation: file, caption: data.PostText, replyMarkup: kb);
                                break;
                            }
                    }
                    data.NextPost = DateTime.UtcNow + TimeSpan.FromMinutes(data.PostInterval);
                    context.Update(data);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error scheduling post: {0}", ex.Message);
            }
        }
    }


    public static void Dispose()
    {
        try
        {
            timer.Stop();
            timer.Dispose();

        }
        catch
        {
            Logging.LogError("unable to stop timers gracefully");
        }
    }
}


