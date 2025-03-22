using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public interface IRoleBusinessLayer
{
    Task<IReadOnlyCollection<GuildRole>> GetGuildRoles(string guildId);
    Task SaveRole(string guildId, ulong roleId);
    Task DeleteRole(string guildId, ulong roleId);
}