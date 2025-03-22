using RoleBot.Models;

namespace RoleBot.DataLayer;

public interface IConfigurationDataLayer
{
    Task<Configuration> GetConfigurationForGuild(ulong guildId, string guildName);
    Task<bool> AddAllowedRoleId(ulong guildId, string guildName, ulong roleId);
    Task<bool> RemoveAllowedRoleId(ulong guildId, string guildName, ulong roleId);
    Task<bool> SetNewUserRole(ulong guildId, string guildName, ulong? roleId);
}