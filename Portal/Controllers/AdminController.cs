using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal.Models.DB;
using Portal.Models.PostModels.Admin;

namespace Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : CustomController
    {
        [Route("Admin/Users")]
        public async Task<ActionResult> Users()
        {
            using (var db = DbHelper.GetDb())
            {
                var users = await db.Users.ToListAsync();

                return View(users);
            }
        }

        [Route("Admin/Roles")]
        public async Task<ActionResult> Roles()
        {
            using (var db = DbHelper.GetDb())
            {
                var roles = await db.Roles.ToListAsync();

                return View(roles);
            }
        }

        [HttpGet]
        [Route("Admin/Roles/Add")]
        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Roles/Add")]
        public async Task<ActionResult> AddRole(AddRoleModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = DbHelper.GetDb())
                {
                    await db.InsertNewRole(new Role()
                    {
                        Description = model.Description,
                        Name = model.Name
                    });
                    return RedirectToAction("Roles");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("Admin/Role/Delete")]
        public async Task<ActionResult> DeleteRole(int roleId)
        {
            using (var db = DbHelper.GetDb())
            {
                var role = await db.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
                db.Roles.Remove(role);
                await db.SaveChangesAsync();
                return RedirectToAction("Roles");
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> Categories()
        {
            using (var db = DbHelper.GetDb())
            {
                var categories = await db.Categories.ToListAsync();
                return View(categories);
            }
            
        }

        [HttpGet]
        public async  Task<ActionResult> Categories(int id)
        {
            return View();
        }
    }
}