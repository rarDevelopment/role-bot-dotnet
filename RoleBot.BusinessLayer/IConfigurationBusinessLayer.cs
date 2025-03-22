using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public interface IConfigurationBusinessLayer
{
    Task<Configuration> GetConfiguration(string guildId, string guildName);
    Task<bool> HasApprovedRole(string guildId, string guildName, IReadOnlyCollection<ulong> roleIds);
    Task<bool> SetApprovedRole(string guildId, string guildName, ulong roleId, bool setAllowed);
    Task<bool> SetNewUserRole(string guildId, string guildName, ulong? roleId);
}