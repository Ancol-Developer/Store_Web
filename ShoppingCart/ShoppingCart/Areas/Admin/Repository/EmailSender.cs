using System.Net;
using System.Net.Mail;

namespace ShoppingCart.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("abcxyz@gmail.com", "abcxyz")
            };

            return client.SendMailAsync(
                new MailMessage(from: "abcxyz@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }

    }
}
