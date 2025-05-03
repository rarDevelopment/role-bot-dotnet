using RoleBot.Models;

namespace RoleBot.DataLayer;

public interface IConfigurationDataLayer
{
    Task<Configuration> GetConfigurationForGuild(string guildId, string guildName);
    Task<bool> AddAllowedRoleId(string guildId, string guildName, ulong roleId);
    Task<bool> RemoveAllowedRoleId(string guildId, string guildName, ulong roleId);
    Task<bool> SetNewUserRole(string guildId, ulong? roleId);
    Task<bool> SetEnableColorChoosing(string guildId, bool isEnabled);
}