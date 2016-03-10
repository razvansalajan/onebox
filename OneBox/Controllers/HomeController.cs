using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace OneBox_WebServices.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        /*
        [Authorize(Roles = "Users")]
        public ActionResult OtherAction()
        {
            return View("Index", GetData("OtherAction"));
        }
        */
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /*
        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> info = new Dictionary<string, object>();
            info.Add("Action", actionName);
            info.Add("User", GetIdentityUser().Name);
            info.Add("Authentificated", GetIdentityUser().IsAuthenticated);
            info.Add("Authnetification type", GetIdentityUser().AuthenticationType);
            info.Add("Is in users role", HttpContext.User.IsInRole("Users"));
            return info;
        }

        private IIdentity GetIdentityUser()
        {
            return HttpContext.User.Identity;
        }*/
    }
}