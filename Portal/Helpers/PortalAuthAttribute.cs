using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Portal.Models;
using Portal.Models.DB;

namespace Portal.Helpers
{
    public class PortalAuthAttribute : AuthorizeAttribute
    {
        public static string ME_SESSION_VAR = "MeUser";

        public User GetCurrentUser(AuthorizationContext context)
        {
            if (HttpContext.Current.Session[ME_SESSION_VAR] == null)
            {
                using (var db = DbHelper.GetDb())
                {
                    var user = db.Users
                        .Include(u => u.Categories.Select(c => c.LinkCategories.Select(lc => lc.Link)))
                        .FirstOrDefault(u => u.UserId == context.HttpContext.User.Identity.Name);
                    HttpContext.Current.Session[ME_SESSION_VAR] = user;
                }
            }

            return (User)HttpContext.Current.Session[ME_SESSION_VAR];
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (AuthorizeCore(filterContext.HttpContext))
            {
                var user = GetCurrentUser(filterContext);
                if (user == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new {controller = "Error", action = "No User"}
                        )
                    );
                }
                else if (!string.IsNullOrEmpty(Roles))
                {
                    if (!user.Roles.Any(r => Roles.Contains(r.Name)))
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                                RouteValueDictionary(
                                    new {controller = "Error", action = "AccessDenied"}
                                )
                        );
                    }
                }
                else if (!String.IsNullOrEmpty(Users))
                {
                    if (!Users.Contains(user.UserId))
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                                RouteValueDictionary(
                                    new {controller = "Error", action = "AccessDenied"}
                                )
                        );
                    }
                }
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}