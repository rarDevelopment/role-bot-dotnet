using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public class ConfigurationBusinessLayer(IConfigurationDataLayer configurationDataLayer) : IConfigurationBusinessLayer
{
    public async Task<Configuration> GetConfiguration(ulong guildId, string guildName)
    {
        return await configurationDataLayer.GetConfigurationForGuild(guildId, guildName);
    }

    public async Task<bool> HasApprovedRole(ulong guildId, string guildName, IReadOnlyCollection<ulong> roleIds)
    {
        var config = await GetConfiguration(guildId, guildName);
        return config.AllowedRoleIds.Any(roleIds.Contains);
    }

    public async Task<bool> SetApprovedRole(ulong guildId, string guildName, ulong roleId, bool setAllowed)
    {
        if (setAllowed)
        {
            return await configurationDataLayer.AddAllowedRoleId(guildId, guildName, roleId);
        }
        return await configurationDataLayer.RemoveAllowedRoleId(guildId, guildName, roleId);
    }

    public async Task<bool> SetNewUserRole(ulong guildId, string guildName, ulong? roleId)
    {
        return await configurationDataLayer.SetNewUserRole(guildId, guildName, roleId);
    }
}