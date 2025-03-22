namespace RoleBot.Models;

public class Configuration
{
    public string GuildId { get; set; }
    public string GuildName { get; set; }
    public List<ulong> AllowedRoleIds { get; set; }
    public ulong? NewUserRole { get; set; }
}