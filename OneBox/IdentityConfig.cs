using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
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
    /// <summary>
    /// Setup the email service in order to be able to send emails to users.
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {

            // Plug in your email service here to send an email.
            // Credentials:
            var credentialUserName = "noreply.onebox@gmail.com";
            var sentFrom = "noreply.onebox@gmail.com";
            var pwd = "testOnebox";

            //var credentialUserName = "onobox-cloud-service@outlook.com";
            //var sentFrom = "onobox-cloud-service@outlook.com";
            //var pwd = "testOnebox";

            // Configure the client:
            SmtpClient client = new SmtpClient("smtp.gmail.com");
                
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            NetworkCredential credentials = new NetworkCredential(credentialUserName, pwd);
            client.EnableSsl = true;
            client.Credentials = credentials;

            // Create the message:
            var mail = new MailMessage(sentFrom, message.Destination);

            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;
            // Send:
            return client.SendMailAsync(mail);
           
        }
    }

    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            //DataProtectionProvider = app.GetDataProtectionProvider();
            //Database.SetInitializer(new IdentityDbInit());
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
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