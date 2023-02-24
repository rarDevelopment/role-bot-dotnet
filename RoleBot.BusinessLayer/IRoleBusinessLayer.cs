using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public interface IRoleBusinessLayer
{
    Task<IReadOnlyCollection<GuildRole>> GetGuildRoles(ulong guildId);
    Task SaveRole(ulong guildId, ulong roleId);
    Task DeleteRole(ulong guildId, ulong roleId);
}