using RoleBot.BusinessLayer;
using DiscordDotNetUtilities.Interfaces;
using RoleBot.Notifications;

namespace RoleBot.EventHandlers;

public class UserJoinedNotificationHandler(IConfigurationBusinessLayer configurationBusinessLayer) : IEventHandler<UserJoinedNotification>
{
    public Task HandleAsync(UserJoinedNotification notification, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            var configuration = await configurationBusinessLayer.GetConfiguration(notification.UserWhoJoined.Guild.Id.ToString(),
                notification.UserWhoJoined.Guild.Name);

            if (configuration.NewUserRole is null or 0)
            {
                return Task.CompletedTask;
            }

            var roleId = Convert.ToUInt64(configuration.NewUserRole);
            var guild = notification.UserWhoJoined.Guild;
            var roles = guild.Roles;
            var roleToAdd = roles.FirstOrDefault(r => r.Id == roleId);
            if (roleToAdd != null)
            {
                await notification.UserWhoJoined.AddRoleAsync(roleToAdd);
            }
            return Task.CompletedTask;
        }, cancellationToken);
        return Task.CompletedTask;
    }
}