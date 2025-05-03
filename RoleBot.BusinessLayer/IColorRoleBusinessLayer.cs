using RoleBot.Models;

namespace RoleBot.BusinessLayer;

public interface IColorRoleBusinessLayer
{
    Task<ColorRole?> GetColorRole(string guildId, string userId);
    Task SaveRole(string guildId, string roleId, string userId);
    Task DeleteRole(string guildId, string roleId, string userId);
}