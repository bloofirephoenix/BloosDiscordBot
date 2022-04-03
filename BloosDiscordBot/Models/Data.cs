using System.Text.Json;

namespace BloosDiscordBot.Models;

public class Data
{
    public static Data? DataInstance { get; set; }

    public ulong? ReactionRoleMessageId { get; set; }

    public static Data? GetData()
    {
        if (File.Exists("data.json"))
        {
            var d = JsonSerializer.Deserialize<Data>(File.ReadAllText("data.json"));
            DataInstance = d;
            return d;
        }

        return null;
    }

    public void SaveData()
    {
        File.WriteAllText("data.json", JsonSerializer.Serialize(this));
    }
}