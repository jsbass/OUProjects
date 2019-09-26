using System.Linq;
using System.Security.Principal;
using Portal.Models.DB;

namespace Portal.Models
{
    public class PortalPrincipal : IPrincipal
    {
        public User User { get; set; }

        public bool IsInRole(string roles)
        {
            return User.Roles.Any(r => roles.Contains(r.Name));
        }

        public PortalPrincipal(User user)
        {
            User = user;
            Identity = new GenericIdentity(User.UserId);
        }

        public IIdentity Identity { get; }
    }
}