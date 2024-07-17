using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface ITransactionHistoryService : IGenericService<TransactionHistory>
    {
        Task Deposit(int accountId, decimal amount, string verificationCode, string description = null);
        Task Withdraw(int accountId, decimal amount, string verificationCode, string description = null);
        Task<string> InitiateDeposit(int accountId, decimal amount, string description = null);
        Task<string> InitiateWithdraw(int accountId, decimal amount, string description = null);
    }
}
