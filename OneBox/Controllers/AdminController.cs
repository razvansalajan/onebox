using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OneBox_WebServices.Infrastructure;
using OneBox_WebServices.Models;
using OneBox_WebServices.Utilities;
using OneBox_WebServices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OneBox_WebServices.Controllers
{
    [AdminRolesAuthorizeAttribute(Utility.AdminRoles)]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = userModel.Name,
                    Email = userModel.Email
                };
                IdentityResult result = await UserManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrosFromResult(result);
                }
            }
            return View(userModel);
        }

        private void AddErrosFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User not found" });
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, string email, string password)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult valuidEmail = await UserManager.UserValidator.ValidateAsync(user);
                if (!valuidEmail.Succeeded)
                {
                    AddErrosFromResult(valuidEmail);
                }
                else
                {
                    IdentityResult validPass = null;
                    if (password != string.Empty)
                    {
                        validPass = await UserManager.PasswordValidator.ValidateAsync(password);
                        if (!validPass.Succeeded)
                        {
                            AddErrosFromResult(validPass);
                        }
                        else
                        {
                            user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                        }
                    }

                    if (validPass == null || (password != string.Empty && validPass.Succeeded))
                    {
                        IdentityResult result = await UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddErrosFromResult(result);
                        }

                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "user not found");
            }
            return View(user);
        }
    }
}