using MediatR;

namespace RoleBot.Notifications;

public class JoinedGuildNotification(SocketGuild joinedGuild) : INotification
{
    public SocketGuild JoinedGuild { get; } = joinedGuild;
}