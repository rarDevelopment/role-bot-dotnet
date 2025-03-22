using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public class RoleBusinessLayer(IRoleDataLayer roleDataLayer) : IRoleBusinessLayer
{
    public async Task<IReadOnlyCollection<GuildRole>> GetGuildRoles(string guildId)
    {
        return await roleDataLayer.GetRolesForGuild(guildId);
    }

    public async Task SaveRole(string guildId, ulong roleId)
    {
        await roleDataLayer.SaveRole(guildId, roleId);
    }

    public async Task DeleteRole(string guildId, ulong roleId)
    {
        await roleDataLayer.DeleteRole(guildId, roleId);
    }
}