using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Portal.Models.Auth;
using Portal.Models.DB;
using Portal.Models.DB.Auth;

namespace Portal.Controllers
{
    public class CustomController : Controller
    {
        protected IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        protected PortalSignInManager _signInManager;
        protected PortalUserManager _userManager;
        protected PortalUserStore _userStore;

        public PortalSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<PortalSignInManager>(); }
            set { _signInManager = value; }
        }

        public PortalUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<PortalUserManager>(); }
            set { _userManager = value; }
        }

        public PortalUserStore UserStore
        {
            get { return _userStore ?? HttpContext.GetOwinContext().Get<PortalUserStore>(); }
        }
    }
}