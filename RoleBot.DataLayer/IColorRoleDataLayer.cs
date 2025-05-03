using RoleBot.Models;

namespace RoleBot.DataLayer;

public interface IColorRoleDataLayer
{
    Task<ColorRole?> GetColorRole(string guildId, string userId);
    Task<bool> SaveColorRole(string guildId, string roleId, string userId);
    Task DeleteColorRole(string guildId, string userId);
}