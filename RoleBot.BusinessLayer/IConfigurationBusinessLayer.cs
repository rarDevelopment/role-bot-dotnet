using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public interface IConfigurationBusinessLayer
{
    Task<Configuration> GetConfiguration(ulong guildId, string guildName);
    Task<bool> HasApprovedRole(ulong guildId, string guildName, IReadOnlyCollection<ulong> roleIds);
    Task<bool> SetApprovedRole(ulong guildId, string guildName, ulong roleId, bool setAllowed);
}