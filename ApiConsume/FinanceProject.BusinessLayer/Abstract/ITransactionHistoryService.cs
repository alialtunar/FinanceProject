using FinanceProject.DtoLayer.Dtos.TransactionHistoryDto;
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

        Task InitiateTransfer(int senderAccountId, string recipientAccountNumber, decimal amount);
        Task Transfer(int senderAccountId, string recipientAccountNumber, decimal amount, string recipientName, string description = null);

        Task<List<TransactionHistory>> TGetLastFiveTransactionsAsync(int accountId);

        Task<decimal> TGetTotalAmountLast24HoursAsync(int accountId);

        Task<IEnumerable<LastTransfersDto>> TGetLast5TransfersUsersAsync(int accountId);

        Task<IEnumerable<TransactionHistory>> TGetPagedTransactionHistoryAsync(int accountId, int page, int pageSize);
    }
}
