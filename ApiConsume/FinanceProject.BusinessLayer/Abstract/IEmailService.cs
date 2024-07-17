namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailAddress, string subject, string body);
    }
}
