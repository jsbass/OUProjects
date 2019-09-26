using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Badges.Helpers;
using Badges.Models;
using Badges.Models.PostModels.Account;
using Badges.Models.PostModels.Badges;

namespace Badges.Controllers
{
    public class BadgeController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(string userId)
        {
            var reqHelper = new RequirementsHelper();
            await reqHelper.UpdateRequirements(userId);

            return RedirectToAction("Index");
        } 

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            using (var db = DbHelper.GetDb())
            {
                var badges = await db.Badges.ToListAsync();
                return View(badges);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var badge = await db.Badges.FirstOrDefaultAsync(b => b.Id == id);
                if (badge != null)
                {
                    db.Badges.Remove(badge);
                }

                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var badge = await db.Badges.Include(b => b.Requirements.Select(r => r.Parameters)).Include(b => b.Image).FirstOrDefaultAsync(b => b.Id == id);
                if (badge != null)
                {
                    return View(badge);
                }
                return Redirect("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            return View(new EditBadge());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EditBadge model)
        {
            if (ModelState.IsValid)
            {
                using (var db = DbHelper.GetDb())
                {
                    var badge = new Badge()
                    {
                        Description = model.Description,
                        Name = model.Name,
                        EstimatedHours = model.EstimatedHours
                    };

                    db.Badges.Add(badge);

                    await db.SaveChangesAsync();

                    return RedirectToAction("Edit", new {id = badge.Id});
                }
            }

            return Redirect("Error");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            using (var db = DbHelper.GetDb())
            {
                var badge = await db.Badges.Include(b => b.Requirements.Select(r => r.Parameters))
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (badge != null)
                {
                    var model = new EditBadge()
                    {
                        Description = badge.Description,
                        Name = badge.Name,
                        Id = badge.Id,
                        EstimatedHours = badge.EstimatedHours ?? 0,
                        ImageSource = badge.Image?.ImageString,
                        Requirements = badge.Requirements.Select(r => new EditRequirement()
                        {
                            Id = r.Id,
                            Text = r.Text,
                            Url = r.UrlForStatus,
                            Params = r.Parameters.Select(p => new EditParam()
                            {
                                Name = p.Name,
                                Value = p.Value
                            }).ToList()

                        }).ToList()
                    };

                    return View(model);
                }
            }

            return Redirect("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditBadge model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id.HasValue)
                {
                    using (var db = DbHelper.GetDb())
                    {
                        var badge = await db.Badges.Include(b => b.Requirements.Select(r => r.Parameters)).FirstOrDefaultAsync(b => b.Id == model.Id);

                        if (badge == null)
                        {
                            return RedirectToAction("Index");
                        }

                        badge.EstimatedHours = model.EstimatedHours;
                        badge.Description = model.Description;
                        badge.Name = model.Name;

                        if (badge.ImageId == null)
                        {
                            badge.Image = new Image();
                        }

                        var matches = Regex.Match(model.ImageSource ?? "", @"data:image/(?<type>.+?),(?<data>.+)");
                        var type = matches.Groups["type"]?.Value ?? "";
                        var data = matches.Groups["data"]?.Value ?? "";
                        
                        badge.Image.ImageData = Convert.FromBase64String(data);
                        badge.Image.Type = type;

                        //Clear deleted requirements
                        foreach (var requirement in badge.Requirements.ToList())
                        {
                            if (!model.Requirements.Any(r => r.Id == requirement.Id))
                            {
                                badge.Requirements.Remove(requirement);
                                foreach (var param in requirement.Parameters.ToList())
                                {
                                    db.Entry(param).State = EntityState.Deleted;
                                }
                                db.Entry(requirement).State = EntityState.Deleted;
                            }
                        }
                        //Add or modify new ones
                        foreach (var requirement in model.Requirements)
                        {
                            var dbReq = badge.Requirements.FirstOrDefault(r => r.Id == requirement.Id) ?? new Requirement();
                            dbReq.Text = requirement.Text;
                            dbReq.UrlForStatus = requirement.Url;
                            //Clear params
                            foreach (var param in dbReq.Parameters.ToList())
                            {
                                dbReq.Parameters.Remove(param);
                                db.Entry(param).State = EntityState.Deleted;;
                            }
                            //Add new ones
                            dbReq.Parameters = requirement.Params.Select(p => new Parameter()
                            {
                                Name = p.Name,
                                Value = p.Value
                            }).ToList();

                            badge.Requirements.Add(dbReq);
                        }

                        await db.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                }
            }

            return View(model);
        }
    }
}