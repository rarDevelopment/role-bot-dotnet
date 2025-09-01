using RoleBot.BusinessLayer;
using DiscordDotNetUtilities.Interfaces;
using RoleBot.Notifications;

namespace RoleBot.EventHandlers;

public class JoinedGuildNotificationHandler(
    IConfigurationBusinessLayer configurationBusinessLayer,
    ILogger<DiscordBot> logger)
    : IEventHandler<JoinedGuildNotification>
{
    public Task HandleAsync(JoinedGuildNotification notification, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            await configurationBusinessLayer.GetConfiguration(notification.JoinedGuild.Id.ToString(), notification.JoinedGuild.Name);
            return Task.CompletedTask;
        }, cancellationToken);
        return Task.CompletedTask;
    }
}