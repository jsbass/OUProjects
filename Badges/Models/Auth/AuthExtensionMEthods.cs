using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Badges.Models.Auth
{
    public static class AuthExtensionMethods
    {
        public static async Task<User> GetUser(this IIdentity identity)
        {
            return await HttpContext.Current.GetOwinContext().GetUserManager<CustomUserManager>().FindByIdAsync(identity.GetUserId());
        }
    }
}