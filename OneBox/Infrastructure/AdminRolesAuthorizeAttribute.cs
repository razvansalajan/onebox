using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneBox_WebServices.Infrastructure
{
    public class AdminRolesAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] roles;
        public AdminRolesAuthorizeAttribute(params string[] role)
        {
            roles = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (string role in roles)
            {
                if (httpContext.User.IsInRole(role))
                {
                    authorize = true;
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
            {
                {"controller", "Account"},
                {"action", "Index"},
                {"returnUrl", filterContext.HttpContext.Request.RawUrl}
            });

        }
    }
}