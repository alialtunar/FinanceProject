using FinanceProject.BusinessLayer.Abstract;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class EmailManager : IEmailService
    {
        public async Task SendEmailAsync(string emailAddress, string subject, string body)
        {
            // Replace with your SMTP settings
            using (var client = new SmtpClient("smtp.gmail.com"))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("altunarali284@gmail.com", "msyvbfcodobzoddp");
                client.Port = 587;
                client.EnableSsl = true;

                var message = new MailMessage("altunarali284@gmail.com", emailAddress, subject, body);
                await client.SendMailAsync(message);
            }
        }
    }
}
