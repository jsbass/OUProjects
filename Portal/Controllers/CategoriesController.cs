using Portal.Models.DB;
using Portal.Models.DB.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;

namespace Portal.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Categories
        public async Task<ActionResult> Index()
        {
            var categories = new List<Category>();
            using (var db = DbHelper.GetDb())
            {
                var user = await User.Identity.GetPortalUser();
                var query = db.Categories.Include(c => c.LinkCategories.Select(lc => lc.Link));

                if (user.IsAdmin())
                {
                    categories = await query.Where(c => c.UserId == user.UserId || c.Global == true).ToListAsync();
                }
                else
                {
                    categories = await query.Where(c => c.UserId == user.UserId).ToListAsync();
                }
            }

            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await User.Identity.GetPortalUser();
            Category category = new Category();
            using (var db = DbHelper.GetDb())
            {
                category = db.Categories.Include(c => c.LinkCategories.Select(lc => lc.Link)).SingleOrDefault(x => x.CategoryId == id);
                if (category.Global == true && user.IsAdmin() == false)
                {
                    //TODO: Make this go to some unauthorized page
                    return View(new Category());
                }
            }

            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        public ActionResult Create(Category cat )// FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Categories/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Categories/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
