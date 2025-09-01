using DiscordDotNetUtilities.Interfaces;
using RoleBot.Notifications;

namespace RoleBot.EventHandlers;

public class MessageReceivedNotificationHandler : IEventHandler<MessageReceivedNotification>
{
    public Task HandleAsync(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            return Task.CompletedTask;
        }, cancellationToken);
        return Task.CompletedTask;
    }
}