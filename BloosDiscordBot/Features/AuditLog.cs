using Discord;
using Discord.WebSocket;

namespace BloosDiscordBot.Features;

public class AuditLog
{
    private ISocketMessageChannel _logChannel;

    public AuditLog(DiscordSocketClient client, ISocketMessageChannel logChannel)
    {
        client.MessageUpdated += OnMessageUpdate;
        client.MessageDeleted += OnMessageDelete;
        client.UserBanned += OnUserBan;
        client.UserUnbanned += OnUserUnban;
        _logChannel = logChannel;
    }

    private async Task OnUserUnban(SocketUser user, SocketGuild guild)
    {
        EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("⚖️ User Unbanned")
            .WithCurrentTimestamp()
            .WithAuthor(user)
            .AddField("Id", user.Id);

        await _logChannel.SendMessageAsync("", false, builder.Build());
    }

    private async Task OnUserBan(SocketUser user, SocketGuild guild)
    {
        EmbedBuilder builder = new EmbedBuilder()
            .WithCurrentTimestamp()
            .WithTitle("🔨 User Banned")
            .WithAuthor(user)
            .AddField("Id", user.Id);

        await _logChannel.SendMessageAsync("", false, builder.Build());
    }

    private async Task OnMessageDelete(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        EmbedBuilder builder = new EmbedBuilder()
            .WithCurrentTimestamp()
            .WithTitle("🗑 Message Deleted");

        if (message.HasValue)
        {
            builder.WithAuthor(message.Value.Author);
            if (message.Value.ToString() != null)
            {
                builder.AddField("Message", message.Value.ToString());
            }
        }

        if (channel.HasValue)
        {
            builder.WithDescription($"In #{channel.Value.Name}");
        }

        await _logChannel.SendMessageAsync("", false, builder.Build());
    }

    private async Task OnMessageUpdate(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("✏️ Message Edited")
            .WithDescription($"In #{channel.Name}")
            .WithAuthor(after.Author)
            .WithCurrentTimestamp();

        if (before.HasValue)
        {
            builder.AddField("Before", before.Value.ToString(), true);
        }
        builder.AddField("After", after.ToString(), true);

        await _logChannel.SendMessageAsync("", false, builder.Build());
    }
}