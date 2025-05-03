using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public class ConfigurationBusinessLayer(IConfigurationDataLayer configurationDataLayer) : IConfigurationBusinessLayer
{
    public async Task<Configuration> GetConfiguration(string guildId, string guildName)
    {
        return await configurationDataLayer.GetConfigurationForGuild(guildId, guildName);
    }

    public async Task<bool> HasApprovedRole(string guildId, string guildName, IReadOnlyCollection<ulong> roleIds)
    {
        var config = await GetConfiguration(guildId, guildName);
        return config.AllowedRoleIds.Any(roleIds.Contains);
    }

    public async Task<bool> SetApprovedRole(string guildId, string guildName, ulong roleId, bool setAllowed)
    {
        if (setAllowed)
        {
            return await configurationDataLayer.AddAllowedRoleId(guildId, guildName, roleId);
        }
        return await configurationDataLayer.RemoveAllowedRoleId(guildId, guildName, roleId);
    }

    public async Task<bool> SetNewUserRole(string guildId, ulong? roleId)
    {
        return await configurationDataLayer.SetNewUserRole(guildId, roleId);
    }

    public async Task<bool> SetEnableColorChoosing(string guildId, bool isEnabled)
    {
        return await configurationDataLayer.SetEnableColorChoosing(guildId, isEnabled);
    }
}