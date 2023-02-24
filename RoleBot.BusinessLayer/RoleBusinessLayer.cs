using RoleBot.DataLayer;
using RoleBot.Models;

namespace RoleBot.BusinessLayer
{
    public class RoleBusinessLayer : IRoleBusinessLayer
    {
        private readonly IRoleDataLayer _roleDataLayer;

        public RoleBusinessLayer(IRoleDataLayer roleDataLayer)
        {
            _roleDataLayer = roleDataLayer;
        }

        public async Task<IReadOnlyCollection<GuildRole>> GetGuildRoles(ulong guildId)
        {
            return await _roleDataLayer.GetRolesForGuild(guildId);
        }

        public async Task SaveRole(ulong guildId, ulong roleId)
        {
            await _roleDataLayer.SaveRole(guildId, roleId);
        }
    }
}
