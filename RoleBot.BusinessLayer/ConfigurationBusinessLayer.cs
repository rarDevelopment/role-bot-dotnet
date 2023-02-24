using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer
{
    public class ConfigurationBusinessLayer : IConfigurationBusinessLayer
    {
        private readonly IConfigurationDataLayer _configurationDataLayer;
        public ConfigurationBusinessLayer(IConfigurationDataLayer configurationDataLayer)
        {
            _configurationDataLayer = configurationDataLayer;
        }

        public async Task<Configuration> GetConfiguration(ulong guildId, string guildName)
        {
            return await _configurationDataLayer.GetConfigurationForGuild(guildId, guildName);
        }

        public async Task<bool> HasApprovedRole(ulong guildId, string guildName, IReadOnlyCollection<ulong> roleIds)
        {
            var config = await GetConfiguration(guildId, guildName);
            return config.AllowedRoleIds.Any(roleIds.Contains);
        }
    }
}
