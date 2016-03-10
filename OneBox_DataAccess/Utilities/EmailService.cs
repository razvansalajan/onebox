using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Utilities
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
}
