using Discord;
using Discord.WebSocket;

namespace BloosDiscordBot.Features;

public class IdeasChannel
{
    private ISocketMessageChannel _channel;
    
    public IdeasChannel(DiscordSocketClient client, ISocketMessageChannel channel)
    {
        _channel = channel;

        client.MessageReceived += OnMessage;
    }

    private async Task OnMessage(SocketMessage msg)
    {
        if (msg.Channel.Id == _channel.Id && msg.Type != MessageType.Reply)
        {
            await msg.AddReactionAsync(new Emoji("❌"));
            await msg.AddReactionAsync(new Emoji("✅"));
        }
    }
}