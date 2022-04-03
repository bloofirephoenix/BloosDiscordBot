using BloosDiscordBot.Models;
using Discord;
using Discord.WebSocket;

namespace BloosDiscordBot.Features;

public class ReactionRoles
{
    private Dictionary<string, IRole> _roles;
    private IMessage _message;
    
    public ReactionRoles(DiscordSocketClient client, ISocketMessageChannel channel, string message,  Dictionary<string, IRole> roles)
    {
        client.ButtonExecuted += OnButtonPressed;

        _roles = roles;

        // create message
        var builder = new ComponentBuilder();

        foreach (var kv in roles)
        {
            builder.WithButton(kv.Key, kv.Key);
        }

        Task.Run(async () =>
        {
            _message = await channel.SendMessageAsync(message, components: builder.Build());
            Data.DataInstance!.ReactionRoleMessageId = _message.Id;
            Data.DataInstance.SaveData();
        }).Wait();
    }

    public ReactionRoles(DiscordSocketClient client, IMessage message,  Dictionary<string, IRole> roles)
    {
        client.ButtonExecuted += OnButtonPressed;
        _roles = roles;
        _message = message;

    }

    private async Task OnButtonPressed(SocketMessageComponent component)
    {
        if (component.Message.Id == _message.Id)
        {
            var id = component.Data.CustomId;

            if (_roles.ContainsKey(id))
            {
                var role = _roles[id];
                var user = await ((IGuildChannel) _message.Channel).Guild.GetUserAsync(component.User.Id);
                
                if (user.RoleIds.Contains(role.Id))
                {
                    await user.RemoveRoleAsync(role);
                }
                else
                {
                    await user.AddRoleAsync(role);
                }
                await component.DeferAsync();
            }
        }
    }
}