using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Portal.Models.DB.Auth
{
    public static class AuthExtensionMEthods
    {
        public static async Task<User> GetPortalUser(this IIdentity identity)
        {
            return await HttpContext.Current.GetOwinContext().GetUserManager<PortalUserManager>().FindByIdAsync(identity.GetUserId());
        }
    }
}