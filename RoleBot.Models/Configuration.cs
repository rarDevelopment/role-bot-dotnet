namespace RoleBot.Models;

public class Configuration
{
    public ulong GuildId { get; set; }
    public string GuildName { get; set; }
    public List<ulong> AllowedRoleIds { get; set; }
}