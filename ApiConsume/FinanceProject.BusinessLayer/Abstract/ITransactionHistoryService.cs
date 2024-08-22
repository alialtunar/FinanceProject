using FinanceProject.Application.Models;
using FinanceProject.ApplicationLayer.Dtos.TransactionHistoryDto;
using FinanceProject.EntityLayer.Concreate;
using System.Net;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface ITransactionHistoryService 
    {
        Task<BaseResponse> Deposit(int accountId, decimal amount, string verificationCode, string description = null);
        Task<BaseResponse> Withdraw(int accountId, decimal amount, string verificationCode, string description = null);
        Task<BaseResponse> InitiateDeposit(int accountId, decimal amount, string description = null);
        Task<BaseResponse> InitiateWithdraw(int accountId, decimal amount, string description = null);

        Task<BaseResponse> InitiateTransfer(int senderAccountId, string recipientAccountNumber, decimal amount);
        Task<BaseResponse> Transfer(int senderAccountId, string recipientAccountNumber, decimal amount, string recipientName, string description = null);

        Task<BaseResponse> TGetLastFiveTransactionsAsync(int accountId);

        Task<BaseResponse> TGetTotalAmountLast24HoursAsync(int accountId);

        Task<BaseResponse> TGetLast5TransfersUsersAsync(int accountId);

        Task<BaseResponse> TGetPagedTransactionHistoryAsync(int accountId, int page, int pageSize);

        Task<BaseResponse> TGetTransactionVolumeLast24Hours();

        Task<BaseResponse> TGetLastFiveTransactionsAsync();

        Task<BaseResponse> TGetLast5TransfersAllUsersAsync();


        Task<BaseResponse> TGetAdminPagedTransactionHistoryAsync(int page, int pageSize);

        public  Task<BaseResponse> TDeleteAsync(int id);


        public  Task<BaseResponse> TGetAllAsync();


        public  Task<BaseResponse> TGetByIdAsync(int id);


        public  Task<BaseResponse> TInsertAsync(TransactionHistory entity);


        public  Task<BaseResponse> TUpdateAsync(TransactionHistory entity);
      




    }
}
