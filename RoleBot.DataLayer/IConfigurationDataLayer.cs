using RoleBot.Models;

namespace RoleBot.DataLayer;

public interface IConfigurationDataLayer
{
    Task<Configuration> GetConfigurationForGuild(ulong guildId, string guildName);
    Task<bool> AddAllowedRoleId(ulong guildId, ulong roleId);
}