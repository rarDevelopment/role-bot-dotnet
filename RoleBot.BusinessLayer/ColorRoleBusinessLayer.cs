using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public class ColorRoleBusinessLayer(IColorRoleDataLayer colorRoleDataLayer) : IColorRoleBusinessLayer
{
    public async Task<ColorRole?> GetColorRole(string guildId, string userId)
    {
        return await colorRoleDataLayer.GetColorRole(guildId, userId);
    }

    public async Task SaveRole(string guildId, string roleId, string userId)
    {
        await colorRoleDataLayer.SaveColorRole(guildId, roleId, userId);
    }

    public async Task DeleteRole(string guildId, string userId)
    {
        await colorRoleDataLayer.DeleteColorRole(guildId, userId);
    }
}