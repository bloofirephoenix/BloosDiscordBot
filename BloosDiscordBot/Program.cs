// See https://aka.ms/new-console-template for more information

using BloosDiscordBot;
using BloosDiscordBot.Features;
using BloosDiscordBot.Models;
using Discord;
using Discord.WebSocket;

Console.WriteLine("BloosDiscordBot");

Config? config = Config.LoadConfig();

Data.GetData();

if (Data.DataInstance == null)
{
    Data.DataInstance = new Data();
}

if (config == null)
{
    Console.WriteLine("Please set the config values in config.json");
    return;
}

var client = new DiscordSocketClient(new DiscordSocketConfig()
{
    MessageCacheSize = 1000
});

client.Log += Logger.Log;

await client.LoginAsync(TokenType.Bot, config.Token);
await client.StartAsync();

client.Ready += async () =>
{
    var guild = client.GetGuild(config.GuildId);

    if (config.AuditLog.Enabled)
    {
        new AuditLog(client, (ISocketMessageChannel) guild.GetChannel(config.AuditLog.ChannelId));
    }

    foreach (var id in config.IdeaChannels)
    {
        new IdeasChannel(client, (ISocketMessageChannel) guild.GetChannel(id));
    }

    if (config.ReactionRoles.Enabled)
    {
        var roles = new Dictionary<string, IRole>();

        foreach (var kv in config.ReactionRoles.Roles)
        {
            roles.Add(kv.Key, guild.GetRole(kv.Value));
        }

        if (Data.DataInstance.ReactionRoleMessageId != null) {
            var message = await ((ISocketMessageChannel) guild.GetChannel(config.ReactionRoles.ChannelId))
                .GetMessageAsync((ulong) Data.DataInstance.ReactionRoleMessageId);

            new ReactionRoles(client, message, roles);
        }
        else
        {
            new ReactionRoles(client, (ISocketMessageChannel)client.GetChannel(config.ReactionRoles.ChannelId), 
                config.ReactionRoles.Message, roles);
        }
    }

    Console.WriteLine("Ready!");
};

await Task.Delay(-1);