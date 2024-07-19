using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class EmailManager : IEmailService
    {
        public async Task SendEmailAsync(string emailAddress, string subject, string body)
        {
            try
            {
              
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
            catch (SmtpException smtpEx)
            {
                // SMTP ile ilgili hatalar
                throw new ErrorException(StatusCodes.Status500InternalServerError, "E-posta gönderilemedi. Lütfen e-posta ayarlarını kontrol edin.");
            }
            catch (Exception ex)
            {
                // Diğer genel hatalar
                throw new ErrorException(StatusCodes.Status500InternalServerError, "E-posta gönderme sırasında bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }
    }
}
