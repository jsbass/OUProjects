using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Badges.Models.Auth
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            using (var client = new SmtpClient())
            {
                var mail = new MailMessage("no-reply@ouprojects.com", message.Destination);
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("no-reply@ouprojects.com", "M0nkeyBusiness!");
                client.Host = "mail.ouprojects.com";
                mail.Subject = message.Subject;
                mail.Body = message.Body;

                await client.SendMailAsync(mail);
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            throw new NotImplementedException();
        }
    }
}