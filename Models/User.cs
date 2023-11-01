using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PublisherBot.Models;


#pragma warning disable CS8618
public class MyUser
{
   public long TelegramId { get; set; }

    public string UserName { get; set; }

}


#pragma warning restore CS8618

