using RoleBot.BusinessLayer;
using RoleBot.Models;

namespace RoleBot.Helpers;

public class RoleHelper
{
    private readonly IRoleBusinessLayer _roleBusinessLayer;
    private readonly IConfigurationBusinessLayer _configurationBusinessLayer;

    public RoleHelper(IRoleBusinessLayer roleBusinessLayer, IConfigurationBusinessLayer configurationBusinessLayer)
    {
        _roleBusinessLayer = roleBusinessLayer;
        _configurationBusinessLayer = configurationBusinessLayer;
    }
    public async Task<bool> CanAdministrate(IGuild guild, IGuildUser guildUser)
    {
        return guildUser.GuildPermissions.Administrator
               || guildUser.GuildPermissions.ManageRoles
               || await _configurationBusinessLayer.HasApprovedRole(guild.Id, guild.Name, guildUser.RoleIds);
    }
    public async Task<bool> IsValidRole(IRole role, IGuild guild)
    {
        var validRoles = await _roleBusinessLayer.GetGuildRoles(guild.Id);
        return guild.Roles.Any(r => r == role) && validRoles.Any(r => r.RoleId == role.Id);
    }
    public IEnumerable<SocketRole> GetValidRoles(SocketGuild guild, IReadOnlyCollection<GuildRole> guildRoles)
    {
        return guild.Roles.Where(r => guildRoles.FirstOrDefault(gr => gr.RoleId == r.Id) != null);
    }
}