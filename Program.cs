using Telegram.Bot;
using Telegram.Bot.Polling;
using PublisherBot.Configuration;
using PublisherBot.Handler;
using Microsoft.Extensions.Logging;
using PublisherBot;
using System.Reflection;

var configuration = Configuration.FromConfig();
if(configuration == null)
{
    Console.WriteLine("Error reading configuration");
    Environment.Exit(0);
}

ILoggerFactory factory = new LoggerFactory();
var logger = factory.CreateLogger<ConversationHandler>();
var bot = new TelegramBotClient(token: configuration.token);
bool terminate = false;
try
{
    var me = await bot.GetMeAsync();
    Logging.LogInfo($"started {me.Username}");

}
catch
{
    Console.WriteLine($"Invalid bot token: {configuration.token}");
    Environment.Exit(0);
}

ReceiverOptions receiverOptions = new ReceiverOptions()
{
    AllowedUpdates = Preferences.AllowedUpdates
};



void DiscoverHandlers()
{
    var assembly = Assembly.GetEntryAssembly();
    var info = assembly!.GetTypes();
    foreach(var type in info)
    {
        if (type.IsClass)
        {
            var attrs = type.GetCustomAttribute<UpdateHandlerAttribute>();
            if (attrs != null)
            {
                if(type.GetInterface(nameof(IMessageUpdateHandler)) != null)
                {
                    var handler = assembly.CreateInstance(type.FullName!) as IMessageUpdateHandler;
                    if (handler != null)
                    {
                        ConversationHandler.AddHanlder(new MessageHanlder(handler));
                    }
                    
                }

                else if (type.GetInterface(nameof(ICallbackQueryUpdateHandler)) != null)
                {
                    var handler = assembly.CreateInstance(type.FullName!) as ICallbackQueryUpdateHandler;
                    if(handler != null)
                    {
                        ConversationHandler.AddHanlder(new CallbackQueryHandler(handler));
                    }
                }
            }
        }
    }
}

DiscoverHandlers();

bot.StartReceiving(
    updateHandler: async (client, update, cancellationToken) =>
    {
        await ConversationHandler.Handle(client, update);
    }, 
    pollingErrorHandler:  (client, exception, cancellationToken) =>
    {

    }, 
    receiverOptions: receiverOptions
    );

Console.CancelKeyPress += (sender, e) =>
{
    terminate = true;
    Scheduler.Dispose();
};

Scheduler.Start(bot);

while (!terminate);
