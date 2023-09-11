using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PublisherBot;

internal class Scheduler
{
    public static System.Timers.Timer timer;
    private static ITelegramBotClient botClient;

    public static void Start(ITelegramBotClient client)
    {
        botClient = client;
        timer = new System.Timers.Timer(2000);
        timer.Enabled = true;
        timer.AutoReset = true;
        timer.Elapsed += ElapsedHandler;
        timer.Start();
       
    }

    public static void ElapsedHandler(object? sender, ElapsedEventArgs e)
    {
        Task.Run(async () =>
        {
            await publish();
        });
    }

    private static async Task publish()
    {
        foreach(var data in ApplicationContext.Posts)
        {
            try
            {
                if (DateTime.UtcNow >= data.NextPost)
                {
                    InputFileId file = new(data.Media);
                    switch (data.MediaType)
                    {
                        case 0:
                            {
                                await botClient.SendPhotoAsync(
                                    data.Chat, photo: file, caption: data.PostText, replyMarkup: data.Markup);
                                break;
                            }
                        case 1:
                            {
                                await botClient.SendVideoAsync(
                                    data.Chat, video: file, caption: data.PostText, replyMarkup: data.Markup);
                                break;
                            }
                        case 2:
                            {
                                await botClient.SendAnimationAsync(
                                    data.Chat, animation: file, caption: data.PostText, replyMarkup: data.Markup);
                                break;
                            }
                    }
                    data.NextPost = DateTime.UtcNow + TimeSpan.FromMinutes(data.PostInterval);
                }
            }
            catch
            {
                // probably the bot was removed from the channel or channel deleted
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