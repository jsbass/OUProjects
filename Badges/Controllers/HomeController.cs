using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Badges.Models;

namespace Badges.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Dashboard()
        {
            using (var db = DbHelper.GetDb())
            {
                var categories = await db.Categories.Include(c => c.Image).Include(c => c.Badges.Select(b => b.Requirements.Select(r => r.Parameters))).Include(c => c.Badges.Select(b => b.Image)).ToListAsync();
                return View(categories);
            }
        }

        public async Task<ActionResult> Badges()
        {
            using (var db = DbHelper.GetDb())
            {
                var categories = await db.Categories.Include(c => c.Image).Include(c => c.Badges.Select(b => b.Requirements.Select(r => r.Parameters))).Include(c => c.Badges.Select(b => b.Image)).ToListAsync();
                return View(categories);
            }
        }

        public async Task<ActionResult> Category(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var category = await db.Categories.Include(c => c.Image).Include(c => c.Badges.Select(b => b.Requirements.Select(r => r.Parameters))).Include(c => c.Badges.Select(b => b.Image)).FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    return Redirect("Error");
                }
                return View(category);
            }
        }

        public async Task<ActionResult> Badge(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var badge = await db.Badges.Include(b => b.Image).Include(b => b.Requirements.Select(r => r.Parameters)).FirstOrDefaultAsync(c => c.Id == id);
                if (badge == null)
                {
                    return Redirect("Error");
                }
                return View(badge);
            }
        }
    }
}