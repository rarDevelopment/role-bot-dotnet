using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public class RoleBusinessLayer(IRoleDataLayer roleDataLayer) : IRoleBusinessLayer
{
    public async Task<IReadOnlyCollection<GuildRole>> GetGuildRoles(ulong guildId)
    {
        return await roleDataLayer.GetRolesForGuild(guildId);
    }

    public async Task SaveRole(ulong guildId, ulong roleId)
    {
        await roleDataLayer.SaveRole(guildId, roleId);
    }

    public async Task DeleteRole(ulong guildId, ulong roleId)
    {
        await roleDataLayer.DeleteRole(guildId, roleId);
    }
}