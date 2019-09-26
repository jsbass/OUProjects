using System.Web;
using System.Web.Mvc;
using Badges.Models;
using Badges.Models.Auth;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Badges.Controllers
{
    public class CustomController : Controller
    {
        protected IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        protected CustomSignInManager _signInManager;
        protected CustomUserManager _userManager;
        protected CustomUserStore _userStore;
        
        public CustomSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<CustomSignInManager>(); }
            set { _signInManager = value; }
        }

        public CustomUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<CustomUserManager>(); }
            set { _userManager = value; }
        }

        public CustomUserStore UserStore
        {
            get { return _userStore ?? HttpContext.GetOwinContext().Get<CustomUserStore>(); }
        }
    }
}