using RoleBot.Models;

namespace RoleBot.DataLayer;

public interface IRoleDataLayer
{
    Task<IReadOnlyCollection<GuildRole>> GetRolesForGuild(string guildId);
    Task<GuildRole?> GetRole(string guildId, ulong roleId);
    Task SaveRole(string guildId, ulong roleId);
    Task DeleteRole(string guildId, ulong roleId);
}