using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Portal.Helpers;
using Portal.Models;
using System.Threading;
using System.Web.Security;
using Portal.Models.DB;
using Portal.Models.DB.Auth;

namespace Portal.Controllers
{

    public class HomeController : CustomController
    {
        public async Task<ActionResult> Index()
        {
            using (var db = DbHelper.GetDb())
            {
                var user = await User.Identity.GetPortalUser();
                var categories = await db.GetCategories(user);
                ViewBag.User = user;
                return View(categories);
            }
        }

        public ActionResult TestModal(string url = "")
        {
            ViewBag.IFrameUrl = url;
            return View();
        }

        [Authorize]
        public async Task<ActionResult> TestEmail(string to)
        {
            using (var client = new SmtpClient())
            {
                var mail = new MailMessage("admin@ouprojects.com", to);
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("admin@ouprojects.com", "M0nkeyBusiness!");
                client.Host = "mail.ouprojects.com";
                mail.Subject = "Test Subject";
                mail.Body = "Test Body";

                await client.SendMailAsync(mail);
            }

            return Content($"Email Sent to {to}");
        }
    }
}