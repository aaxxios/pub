using Telegram.Bot.Types.ReplyMarkups;

namespace PublisherBot.Models
{
    public class Post
    {
#pragma warning disable CS8618

        public int Id { get; set; }
        public string PostText { get; set; }
        public string Media { get; set; }
        public short MediaType { get; set; }

        public string Markup { get; set; }

        public int PostInterval { get; set; }

        public DateTime NextPost { get; set; }
        public long Chat { get; set; }
    }
}
