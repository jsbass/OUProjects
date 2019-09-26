using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Portal.Helpers;
using Portal.Models;
using Portal.Models.DB;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Portal.Models.DB.Auth;
using Portal.Models.PostModels.Account;
using WebGrease.Css.Extensions;

namespace Portal.Controllers
{
    public class AccountController : CustomController
    {
        [Authorize]
        [HttpGet]
        // GET: Account/Edit?userId={userId}
        public async Task<ActionResult> Edit(string userId = "")
        {
            ViewBag.IsAdmin = await UserManager.IsInRoleAsync(User.Identity.Name, "Admin");
            User user = null;
            if ((bool) ViewBag.IsAdmin && !string.IsNullOrEmpty(userId))
            {
                user = UserManager.Users.FirstOrDefault(u => u.UserId == userId);
            }
            else
            {
                user = await User.Identity.GetPortalUser();
            }
            if (user != null)
            {
                using (var db = DbHelper.GetDb())
                {
                    ViewBag.Roles = await db.Roles.OrderBy(r => r.RoleId).ToListAsync();
                }
                return View(new EditModel()
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleIds = user.Roles.Select(r => r.Id).ToList()
                });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditModel model)
        {
            if (ModelState.IsValid)
            {
                var isAdmin = await UserManager.IsInRoleAsync(User.Identity.Name, "Admin");
                if (model.UserId == User.Identity.GetUserId() || isAdmin)
                {
                    var user = await UserManager.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);
                    IdentityResult result;
                    if (isAdmin)
                    {
                        string[] roleNames = new string[model.RoleIds.Count];
                        using (var db = DbHelper.GetDb())
                        {
                            roleNames = await db.Roles.Where(r => model.RoleIds.Contains(r.RoleId)).Select(r => r.Name).ToArrayAsync();
                        }

                        result = await UserManager.RemoveFromRolesAsync(user.UserId, user.Roles.Select(r => r.Name).ToArray());
                        if(!result.Succeeded) result.Errors.ForEach(e => ModelState.AddModelError("Roles", e));

                        await UserManager.AddToRolesAsync(user.UserId, roleNames);
                        if (!result.Succeeded) result.Errors.ForEach(e => ModelState.AddModelError("Roles", e));
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    result = await UserManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            result = await UserManager.PasswordValidator.ValidateAsync(model.Password);
                            if (result.Succeeded)
                            {
                                result =
                                    await
                                        UserManager.ChangePasswordAsync(model.UserId, model.CurrentPassword,
                                            model.Password);
                                if (result.Succeeded)
                                {
                                    return RedirectToAction("Edit",
                                        new {userId = User.Identity.GetUserId() == model.UserId ? model.UserId : ""});
                                }
                                else
                                {
                                    result.Errors.ForEach(e => ModelState.AddModelError("Password", e));
                                }
                            }
                            else
                            {
                                result.Errors.ForEach(e => ModelState.AddModelError("Password", e));
                            }
                        }
                        else
                        {
                            return RedirectToAction("Edit", new { userId = User.Identity.GetUserId() == model.UserId ? model.UserId : "" });
                        }
                    }
                    else
                    {
                        result.Errors.ForEach(e => ModelState.AddModelError("", e));
                    }
                }
            }

            using (var db = DbHelper.GetDb())
            {
                ViewBag.Roles = await db.Roles.OrderBy(r => r.RoleId).ToListAsync();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(LogInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                SignInStatus result;
                if (user == null)
                {
                    result = SignInStatus.Failure;
                }
                else
                {
                    result =
                    await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, true);
                }
                
                switch (result)
                {
                    case SignInStatus.Success:
                        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                        return RedirectToAction("Index", "Home");
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("","Account Locked Out");
                        break;
                    case SignInStatus.RequiresVerification:
                        ModelState.AddModelError("","Requires Verification");
                        break;
                    case SignInStatus.Failure:
                        //same as default
                    default:
                        ModelState.AddModelError("", "Invalid Login");
                        break;
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    UserId = model.Email.Replace("@ou.edu","")
                };

                if ((await UserManager.FindByEmailAsync(user.Email)) == null)
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SendConfirmationEmail(user.UserId);

                        return RedirectToAction("ResendEmailConfirmationCode");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("","User with that email already exists");
                }

                
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ResendEmailConfirmationCode()
        {
            return View(new SendEmailConfirmModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendEmailConfirmationCode(SendEmailConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByEmail(model.Email);
                if (user != null)
                {
                    await SendConfirmationEmail(user.UserId);
                }

                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId != null && code != null)
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View("Error");
        }

        private async Task SendConfirmationEmail(string userId)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var url = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code },
                protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Confirm Account",
                    $@"Click <a href=""{url}"">here</a> to confirm your email");
        }
    }
}