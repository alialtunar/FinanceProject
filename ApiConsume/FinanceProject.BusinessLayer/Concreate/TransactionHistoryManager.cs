using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class TransactionHistoryManager : ITransactionHistoryService
    {
        private readonly ITransactionHistoryDal _transactionHistoryDal;
        private readonly IAccountService _accountService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IUserService _userService;

        public TransactionHistoryManager(ITransactionHistoryDal transactionHistoryDal, IAccountService accountService, IVerificationCodeService verificationCodeService,IUserService userService)
        {
            _transactionHistoryDal = transactionHistoryDal;
            _accountService = accountService;
            _verificationCodeService = verificationCodeService;
            _userService = userService;
        }

        public async Task<string> InitiateDeposit(int accountId, decimal amount, string description = null)
        {
            var verificationCode = await _verificationCodeService.CreateVerificationCodeAsync(accountId, amount, TransactionType.Deposit);
            // Send email to user with the code
            // SendEmailToUser(accountId, verificationCode.Code);
            return verificationCode.Code;
        }

        public async Task<string> InitiateWithdraw(int accountId, decimal amount, string description = null)
        {
            var account = await _accountService.TGetByIdAsync(accountId);
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }
            var verificationCode = await _verificationCodeService.CreateVerificationCodeAsync(accountId, amount, TransactionType.Withdrawal);
            // Send email to user with the code
            // SendEmailToUser(accountId, verificationCode.Code);
            return verificationCode.Code;
        }

        public async Task Deposit(int accountId, decimal amount, string verificationCode, string description = null)
        {
            if (!await _verificationCodeService.VerifyCodeAsync(accountId, verificationCode, amount, TransactionType.Deposit))
            {
                throw new InvalidOperationException("Invalid or expired verification code.");
            }

            var account = await _accountService.TGetByIdAsync(accountId);
            account.Balance += amount;
            await _accountService.TUpdateAsync(account);

            var transaction = new TransactionHistory
            {
                AccountID = accountId,
                Amount = amount,
                TransactionType = TransactionType.Deposit,
                TransactionDate = DateTime.Now,
                Description = description
            };
            await _transactionHistoryDal.InsertAsync(transaction);
        }

        public async Task Withdraw(int accountId, decimal amount, string verificationCode, string description = null)
        {
            if (!await _verificationCodeService.VerifyCodeAsync(accountId, verificationCode, amount, TransactionType.Withdrawal))
            {
                throw new InvalidOperationException("Invalid or expired verification code.");
            }

            var account = await _accountService.TGetByIdAsync(accountId);
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }
            account.Balance -= amount;
            await _accountService.TUpdateAsync(account);

            var transaction = new TransactionHistory
            {
                AccountID = accountId,
                Amount = amount,
                TransactionType = TransactionType.Withdrawal,
                TransactionDate = DateTime.Now,
                Description = description
            };
            await _transactionHistoryDal.InsertAsync(transaction);
        }


        public async Task InitiateTransfer(int senderAccountId, string recipientAccountNumber, decimal amount)
        {
            var senderAccount = await _accountService.TGetByIdAsync(senderAccountId);
            if (senderAccount.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            var recipientAccount = await _accountService.GetByAccountNumberAsync(recipientAccountNumber);
            if (recipientAccount == null)
            {
                throw new InvalidOperationException("Recipient account not found.");
            }
        }

        public async Task Transfer(int senderAccountId, string recipientAccountNumber, decimal amount, string recipientName, string description = null)
        {
            var senderAccount = await _accountService.TGetByIdAsync(senderAccountId);
            if (senderAccount.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            var recipientAccount = await _accountService.GetByAccountNumberAsync(recipientAccountNumber);
            if (recipientAccount == null)
            {
                throw new InvalidOperationException("Recipient account not found.");
            }

            var recipientUser = await _userService.TGetByIdAsync(recipientAccount.UserID);
            var fullName = recipientUser.FullName.ToLower();
            if (fullName != recipientName.ToLower())
            {
                throw new InvalidOperationException("Recipient information does not match.");
            }

            // Para transferini gerçekleştirme
            senderAccount.Balance -= amount;
            recipientAccount.Balance += amount;
            await _accountService.TUpdateAsync(senderAccount);
            await _accountService.TUpdateAsync(recipientAccount);

            var transaction = new TransactionHistory
            {
                AccountID = senderAccountId,
                Amount = amount,
                TransactionType = TransactionType.Transfer,
                TransactionDate = DateTime.Now,
                RecipientAccountNumber = recipientAccountNumber,
                RecipientName = recipientName,
                Description = description
            };
            await _transactionHistoryDal.InsertAsync(transaction);
        }



        public async Task TDeleteAsync(int id)
        {
            await _transactionHistoryDal.DeleteAsync(id);
        }

        public async Task<List<TransactionHistory>> TGetAllAsync()
        {
            return await _transactionHistoryDal.GetAllAsync();
        }

        public async Task<TransactionHistory> TGetByIdAsync(int id)
        {
            return await _transactionHistoryDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(TransactionHistory entity)
        {
            await _transactionHistoryDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(TransactionHistory entity)
        {
            await _transactionHistoryDal.UpdateAsync(entity);
        }
    }
}
