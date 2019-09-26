using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Badges.Models.Auth
{
    public class CustomRoleManager : RoleManager<Role, int>
    {
        public CustomRoleManager(IRoleStore<Role, int> store) : base(store)
        {
        }

        public static CustomRoleManager Create(IdentityFactoryOptions<CustomRoleManager> options, IOwinContext context)
        {
            return null;
        }
    }
}