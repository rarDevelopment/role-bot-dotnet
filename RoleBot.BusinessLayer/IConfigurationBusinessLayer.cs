using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public interface IConfigurationBusinessLayer
{
    Task<Configuration> GetConfiguration(ulong guildId, string guildName);
    Task<bool> HasApprovedRole(ulong guildId, string guildName, IReadOnlyCollection<ulong> roleIds);
}