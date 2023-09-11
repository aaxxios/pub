using Tomlyn;
namespace PublisherBot.Configuration;

#pragma warning disable CS8618
public class Configuration
{
    public  List<long> permittedusers { get; set; }
    public  string token { get; set; }

    public static Configuration Instance { get; set; }
    public static Configuration? FromConfig()
    {
        try
        {
            using(StreamReader reader = new("settings.toml"))
            {
                Instance = Toml.ToModel<Configuration>(reader.ReadToEnd(), options: new TomlModelOptions()
                {
                    
                });
                return Instance;
            }
        }
        catch(Exception ex) 
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

}

#pragma warning restore CS8618