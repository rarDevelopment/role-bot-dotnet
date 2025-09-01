namespace RoleBot.Notifications;

public class JoinedGuildNotification(SocketGuild joinedGuild)
{
    public SocketGuild JoinedGuild { get; } = joinedGuild;
}