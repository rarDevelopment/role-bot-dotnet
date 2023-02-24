using RoleBot.Models;

namespace RoleBot.DataLayer;

public interface IRoleDataLayer
{
    Task<IReadOnlyCollection<GuildRole>> GetRolesForGuild(ulong guildId);
    Task<GuildRole?> GetRole(ulong guildId, ulong roleId);
    Task SaveRole(ulong guildId, ulong roleId);
    Task DeleteRole(ulong guildId, ulong roleId);
}