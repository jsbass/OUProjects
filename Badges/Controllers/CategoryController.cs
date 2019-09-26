using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Badges.Models;
using Badges.Models.PostModels.Badges;

namespace Badges.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            using (var db = DbHelper.GetDb())
            {
                var categories = await db.Categories.Include(c => c.Image).Include(c => c.Badges.Select(b => b.Requirements.Select(r => r.Parameters))).ToListAsync();
                return View(categories);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            using (var db = DbHelper.GetDb())
            {
                ViewBag.SelectListItems = await db.Badges.Select(b => new SelectListItem()
                {
                    Text = b.Name,
                    Value = b.Id.ToString()
                }).ToListAsync();
                return View(new EditCategory());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EditCategory model)
        {
            if (ModelState.IsValid)
            {
                using (var db = DbHelper.GetDb())
                {
                    var badges = await db.Badges.Where(b => model.BadgeIds.Contains(b.Id)).ToListAsync();
                    var category = new Category()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Badges = badges
                    };

                    db.Categories.Add(category);

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = model.Id });
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var category = await db.Categories.Include(c => c.Badges.Select(b => b.Requirements.Select(r => r.Parameters))).Include(c => c.Image).FirstOrDefaultAsync(c => c.Id == id);
                return View(category);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var category = await db.Categories.Include(c => c.Badges).Include(c => c.Image).FirstOrDefaultAsync(c => c.Id == id);
                
                ViewBag.SelectListItems = (await db.Badges.ToListAsync()).Select(b => new SelectListItem()
                {
                    Selected = category.Badges.Contains(b),
                    Text = b.Name,
                    Value = b.Id.ToString()
                });
                return View(new Badges.Models.PostModels.Badges.EditCategory()
                {
                    Id = category.Id,
                    Name = category.Name,
                    BadgeIds = category.Badges.Select(b => b.Id),
                    Description = category.Description,
                    ImageSource = category.Image?.ImageString ?? ""
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditCategory model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id.HasValue)
                {
                    using (var db = DbHelper.GetDb())
                    {
                        var badges = db.Badges.Where(b => model.BadgeIds.Contains(b.Id));
                        var category = new Category();

                        if (model.Id.HasValue)
                        {
                            category = await db.Categories.FirstOrDefaultAsync(c => c.Id == model.Id);
                        }

                        if (category.ImageId == null)
                        {
                            category.Image = new Image();
                        }


                        var matches = Regex.Match(model.ImageSource ?? "", @"data:image/(?<type>.+?),(?<data>.+)");
                        var type = matches.Groups["type"]?.Value ?? "";
                        var data = matches.Groups["data"]?.Value ?? "";

                        category.Image.ImageData = Convert.FromBase64String(data);
                        category.Image.Type = type;

                        category.Badges.Clear();
                        foreach (var badge in badges)
                        {
                            category.Badges.Add(badge);
                        }

                        category.Name = model.Name;
                        category.Description = model.Description;

                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { id = model.Id });
                    }
                }

                ModelState.AddModelError("", "Id does not have value");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveBadge(int id, int badgeId)
        {
            using (var db = DbHelper.GetDb())
            {
                var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                var badge = category.Badges.FirstOrDefault(b => b.Id == badgeId);
                if (badge != null)
                {
                    category.Badges.Remove(badge);
                }

                await db.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new {id = id});
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                db.Categories.Remove(category);

                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}