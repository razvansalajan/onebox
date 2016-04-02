using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using OneBox_DataAccess.DatabaseContexts;
using OneBox_DataAccess.Infrastucture;
using OneBox_WebServices.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace OneBox_WebServices
{
    

    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            //DataProtectionProvider = app.GetDataProtectionProvider();
            //Database.SetInitializer(new IdentityDbInit());
            app.CreatePerOwinContext<ApplicationDbContext>(ApplicationDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            // look over how this works.
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseGoogleAuthentication(clientId: "531756177323-22mt5oveklsf9f5vpd8fm2r3juuai450.apps.googleusercontent.com", clientSecret: "REAAZjLqSvkuVY3I8g2MXTZn");
        }
    }
}