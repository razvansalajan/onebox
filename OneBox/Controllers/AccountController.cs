using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OneBox_WebServices.Infrastructure;
using OneBox_WebServices.Models;
using OneBox_WebServices.Utilities;
using OneBox_WebServices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OneBox_WebServices.Controllers
{
    [Authorize]
    /// Display the info for the user after authentification.
    public class AccountController : Controller
    {
        /// <summary>
        /// Returns what the user can do after authentification.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(GetInfoData());
        }

        /// <summary>
        /// The login action. 
        /// If the user has already tried to log in and failed the error are going to be sent to the view.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>A view for inserting the credential</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                /// If already is loged in then redirect to prinpal action.
                return RedirectToAction("Index");
            }

            ///TODO: This should be changed. The return url
            ViewBag.Url = returnUrl;
            ///TODO:This should be changed.
            AddErrosFromResult((IdentityResult)TempData["Errors"]);
            return View();
        }
        /// <summary>
        /// Call the middleware login authentification service.
        /// </summary>
        /// <param name="returnUrl">The return path.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback", new { returnUrl = returnUrl })
            };
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
            ///TODO:De ce dau return asta ?
            return new HttpUnauthorizedResult();
        }

        /// <summary>
        /// The action that Google calls back with user credential
        /// TODO : ce se intampla daca nu poate sa se logheze cu google ? sau daca is creeaza cont.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>redirect to Index or Login(again)</returns>
        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();

            ///Find the user. Maybe has already login with google before.
            AppUser user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                /// If this the first time when the user has logged in with google on "onebox". 
                user = new AppUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.DefaultUserName,
                };

                ///Create new user.
                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    /// If the creation has not succeeded( for exemple another user exist with the same email).
                    /// Save the error.
                    TempData["Errors"] = result;
                    /// Redirect to Login action(and display the erros).
                    return RedirectToAction("Login");
                }
                else {
                    ///Try to add login to the created user.
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        /// Save the errors and redirect to Login action.
                        TempData["Errors"] = result;
                        return RedirectToAction("Login");
                    }
                }
            }
            /// TODO: tre sa vad mai clar ce se intampla pe aici ( adica mai sus face addlogin async).
            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            ident.AddClaims(loginInfo.ExternalIdentity.Claims);
            AuthManager.SignOut();
            AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

            ///It is a simple user so add the user to "Users" role.
            if (!UserManager.IsInRole(user.Id, Utility.UsersRole))
            {
                UserManager.AddToRole(user.Id, Utility.UsersRole);
            }

            /// Redirect to main page of the authentificated user.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtain the current User.Identity from the current REQUEST !!!!.
        /// </summary>
        /// <returns></returns>
        private IIdentity GetIdentityUser()
        {
            return HttpContext.User.Identity;
        }

        /// <summary>
        /// Get some basic data of the authentifacted user.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetInfoData()
        {
            Dictionary<string, object> info = new Dictionary<string, object>();
            info.Add("User", GetIdentityUser().Name);
            info.Add("Authentificated", GetIdentityUser().IsAuthenticated);
            info.Add("Authnetification type", GetIdentityUser().AuthenticationType);
            var id = GetIdentityUser().GetUserId();

            var user = UserManager.FindById(id);

            info.Add("email", user.Email);
            info.Add("Is in users role", HttpContext.User.IsInRole("Users"));
            return info;
        }

        /// <summary>
        /// Try to create a new user.
        /// </summary>
        /// <param name="userModel">The info of the user.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserViewModel userModel)
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
                    user = UserManager.Find(userModel.Name, userModel.Password);
                    UserManager.AddToRole(user.Id, Utility.UsersRole);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrosFromResult(result);
                }
            }
            return View();
        }


        private void AddErrosFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //check if the input from the user is in valid state.
                AppUser user = await UserManager.FindAsync(details.Name, details.Password);
                if (user == null)
                {
                    // if the user doen not exist.
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    // user exist.
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                    // user has signed in.
                    // redirect to previous page.
                    if (returnUrl == null || returnUrl == string.Empty)
                    {
                        //GetInfoData();
                        //returnView("Index", GetInfoData());
                        return RedirectToAction("Index");
                    }
                    else {
                        //GetInfoData();
                        return Redirect(returnUrl);
                    }
                }
            }
            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }

}