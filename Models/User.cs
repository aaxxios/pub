using System.ComponentModel.DataAnnotations;

namespace PublisherBot.Models;


#pragma warning disable CS8618
public class MyUser
{
    public int Id { get; set; }
    public string Balance { get; set; }

}


public class Transaction
{
    public int Id { get; set; }

    public MyUser User { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public bool Confirmed { get; set; }

}

#pragma warning restore CS8618

