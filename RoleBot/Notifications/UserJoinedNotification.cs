using MediatR;

namespace RoleBot.Notifications;

public class UserJoinedNotification(SocketGuildUser userWhoJoined) : INotification
{
    public SocketGuildUser UserWhoJoined { get; } = userWhoJoined;
}