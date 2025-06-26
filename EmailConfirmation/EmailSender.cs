using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.Xml;

namespace ECommerceAPI.EmailConfirmation
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var myEmail = "dev.mohamed.ahmed.2001@gmail.com";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(myEmail, "wxqtlfdhtsconpwt"),
                EnableSsl = true
            };

            var mail = new MailMessage(myEmail, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mail);

        }

    }
}
