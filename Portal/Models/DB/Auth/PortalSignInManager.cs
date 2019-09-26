using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer.Utilities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Portal.Models.DB.Auth
{
    public class PortalSignInManager : SignInManager<User, string>
    {
        public PortalSignInManager(PortalUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override async Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            var id = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            id.AddClaim(new Claim("Id", user.UserId));
            return id;
        }

        public static PortalSignInManager Create(IdentityFactoryOptions<PortalSignInManager> options, IOwinContext context)
        {
            return new PortalSignInManager(context.GetUserManager<PortalUserManager>(), context.Authentication);
        }
    }
}