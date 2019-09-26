using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Portal.Models.Auth;

namespace Portal.Models.DB.Auth
{
    public class PortalRoleManager : RoleManager<Role, int>
    {
        public PortalRoleManager(IRoleStore<Role, int> store) : base(store)
        {
        }

        public static PortalRoleManager Create(IdentityFactoryOptions<PortalRoleManager> options, IOwinContext context)
        {
            return null;
        }
    }
}