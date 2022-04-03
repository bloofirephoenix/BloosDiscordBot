using System.Text.Json;
namespace BloosDiscordBot.Models;

public class Config
{
    public ulong GuildId { get; set; } = 0;
    public string Token { get; set; } = "";
    public Audit AuditLog { get; set; } = new();
    public class Audit
    {
        public bool Enabled { get; set; } = false;
        public ulong ChannelId { get; set; } = 0;
    }
    public ButtonRoles ReactionRoles { get; set; } = new();
    public class ButtonRoles
    {
        public bool Enabled { get; set; } = false;
        public string Message { get; set; } = "";
        public Dictionary<string, ulong> Roles { get; set; } = new();
        public ulong ChannelId { get; set; } = 0;
    }

    public List<ulong> IdeaChannels { get; set; } = new();

    public static Config? LoadConfig()
    {
        if (File.Exists("config.json"))
        {
            var c = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
            if (c != null)
            {
                File.WriteAllText("config.json", JsonSerializer.Serialize(c, new JsonSerializerOptions() { WriteIndented = true }));
            }

            return c;
        }

        File.WriteAllText("config.json" ,JsonSerializer.Serialize(new Config()));

        return null;
    }
}