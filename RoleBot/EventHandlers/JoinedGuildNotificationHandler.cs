using MediatR;
using RoleBot.BusinessLayer;
using RoleBot.Notifications;

namespace RoleBot.EventHandlers;
public class JoinedGuildNotificationHandler(
    IConfigurationBusinessLayer configurationBusinessLayer,
    ILogger<DiscordBot> logger)
    : INotificationHandler<JoinedGuildNotification>
{
    public Task Handle(JoinedGuildNotification notification, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            await configurationBusinessLayer.GetConfiguration(notification.JoinedGuild.Id.ToString(), notification.JoinedGuild.Name);
            return Task.CompletedTask;
        }, cancellationToken);
        return Task.CompletedTask;
    }
}