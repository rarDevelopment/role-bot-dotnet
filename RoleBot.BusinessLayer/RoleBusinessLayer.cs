using RoleBot.DataLayer;

namespace RoleBot.BusinessLayer
{
    public class RoleBusinessLayer : IRoleBusinessLayer
    {
        private readonly IRoleDataLayer _roleDataLayer;

        public RoleBusinessLayer(IRoleDataLayer roleDataLayer)
        {
            _roleDataLayer = roleDataLayer;
        }
    }
}
