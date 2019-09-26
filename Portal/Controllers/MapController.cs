using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Portal.Models.DB;
using Portal.Models.DB.Auth;

namespace Portal.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult Index(string locationCode = "")
        {
            if (locationCode == null) locationCode = "";
            ViewBag.LocationCode = locationCode;
            var id = User.Identity.Name;
            var um = HttpContext.GetOwinContext().GetUserManager<PortalUserManager>();
            ViewBag.CanEdit = !string.IsNullOrWhiteSpace(id) &&
                              (um.IsInRole(id, "Admin") || um.IsInRole(id, "MapEditor"));
            return View();
        }

        public ActionResult GoToBuilding(string ouCode)
        {
            using (var db = DbHelper.GetDb())
            {
                var building = db.Buildings.Include(b => b.Location).FirstOrDefault(b => b.BuildingCode.Equals(ouCode, StringComparison.InvariantCultureIgnoreCase));
                return RedirectToAction("Index", new {locationCode = building?.Location?.Id});
            }
        }

        public ActionResult GoToParking(string ouCode)
        {
            using (var db = DbHelper.GetDb())
            {
                var parking = db.Parkings.Include(b => b.Location).FirstOrDefault(b => b.ParkingCode.Equals(ouCode, StringComparison.InvariantCultureIgnoreCase));
                return RedirectToAction("Index", new { locationCode = parking?.Location?.Id });
            }
        }

        [Authorize(Roles = "Admin,MapEditor")]
        public ActionResult Edit(string locationCode = "")
        {
            ViewBag.LocationCode = locationCode;
            return View();
        }
    }
}