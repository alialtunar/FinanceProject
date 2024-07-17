using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IVerificationCodeService : IGenericService<VerificationCode>
    {
        Task<VerificationCode> CreateVerificationCodeAsync(int accountId, decimal amount, TransactionType transactionType);
        Task<bool> VerifyCodeAsync(int accountId, string code, decimal amount, TransactionType transactionType);
    }
}
