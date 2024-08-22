using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.ApplicationLayer.Exceptions;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.ApplicationLayer.Dtos.TransactionHistoryDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using FinanceProject.Application.Models;
using System.Net;

public class TransactionHistoryManager : ITransactionHistoryService
{
    private readonly ITransactionHistoryDal _transactionHistoryDal;
    private readonly IAccountService _accountService;
    private readonly IVerificationCodeService _verificationCodeService;
    private readonly IUserService _userService;
    private readonly BaseResponse _response;

    public TransactionHistoryManager(ITransactionHistoryDal transactionHistoryDal, IAccountService accountService, IVerificationCodeService verificationCodeService, IUserService userService, BaseResponse response)
    {
        _transactionHistoryDal = transactionHistoryDal;
        _accountService = accountService;
        _verificationCodeService = verificationCodeService;
        _userService = userService;
        _response = response;
    }

    public async Task<BaseResponse> TGetLastFiveTransactionsAsync(int accountId)
    {
        
        try
        {
            var transactions = await _transactionHistoryDal.GetLastFiveTransactionsAsync(accountId);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Son beş işlem alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> InitiateDeposit(int accountId, decimal amount, string description = null)
    {
        
        try
        {
            var verificationCode = await _verificationCodeService.CreateVerificationCodeAsync(accountId, amount, TransactionType.Deposit);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = verificationCode.Code;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Para yatırma işlemi başlatılamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> InitiateWithdraw(int accountId, decimal amount, string description = null)
    {
        try
        {
            var accountResponse = await _accountService.TGetByIdAsync(accountId);
            var account = accountResponse.Result as Account;
            if (account.Balance < amount)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.ErrorMessages.Add("Yetersiz bakiye.");
                return _response;
            }

            var verificationCode = await _verificationCodeService.CreateVerificationCodeAsync(accountId, amount, TransactionType.Withdrawal);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = verificationCode.Code; // Ensure this is a string and not an object
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Para çekme işlemi başlatılamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }


    public async Task<BaseResponse> Deposit(int accountId, decimal amount, string verificationCode, string description = null)
    {
       
        try
        {
            if (!await _verificationCodeService.VerifyCodeAsync(accountId, verificationCode, amount, TransactionType.Deposit))
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.ErrorMessages.Add("Geçersiz veya süresi dolmuş doğrulama kodu.");
                return _response;
            }

            var accountResponse = await _accountService.TGetByIdAsync(accountId);
            var account = accountResponse.Result as Account;
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

            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "Para yatırma işlemi başarılı.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Para yatırma işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> Withdraw(int accountId, decimal amount, string verificationCode, string description = null)
    {
       
        try
        {
            if (!await _verificationCodeService.VerifyCodeAsync(accountId, verificationCode, amount, TransactionType.Withdrawal))
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.ErrorMessages.Add("Geçersiz veya süresi dolmuş doğrulama kodu.");
                return _response;
            }

            var accountResponse = await _accountService.TGetByIdAsync(accountId);
            var account = accountResponse.Result as Account;
            if (account.Balance < amount)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.ErrorMessages.Add("Yetersiz bakiye.");
                return _response;
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

            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "Para çekme işlemi başarılı.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Para çekme işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
        return _response;
    }
    public async Task<BaseResponse> InitiateTransfer(int senderAccountId, string recipientAccountNumber, decimal amount)
    {
        try
        {
            var senderAccountResponse = await _accountService.TGetByIdAsync(senderAccountId);
            var senderAccount = senderAccountResponse.Result as Account;
            if (senderAccount.Balance < amount)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.ErrorMessages.Add("Yetersiz bakiye.");
                return _response;
            }

            var recipientAccountResponse = await _accountService.TGetByAccountNumberAsync(recipientAccountNumber);
            if (recipientAccountResponse.Result == null)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Alıcı bulunamadı.");
                return _response;
            }

            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "Transfer işlemi başlatıldı.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Transfer işlemi başlatılamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> Transfer(int senderAccountId, string recipientAccountNumber, decimal amount, string recipientName, string description = null)
    {
        try
        {
            var senderAccountResponse = await _accountService.TGetByIdAsync(senderAccountId);
            var senderAccount = senderAccountResponse.Result as Account;
            if (senderAccount.Balance < amount)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.Forbidden;
                _response.ErrorMessages.Add("Yetersiz bakiye.");
                return _response;
            }

            var recipientAccountResponse = await _accountService.TGetByAccountNumberAsync(recipientAccountNumber);
            var recipientAccount = recipientAccountResponse.Result as Account;
            if (recipientAccount == null)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Alıcı bulunamadı.");
                return _response;
            }

            var recipientUserResponse = await _userService.TGetByIdAsync(recipientAccount.UserID);
            var recipientUser = recipientUserResponse.Result as User;

            if (recipientUser == null)
            {
                _response.isSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Alıcı kullanıcı bulunamadı.");
                return _response;
            }

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
                Description = description
            };
            await _transactionHistoryDal.InsertAsync(transaction);

            transaction = new TransactionHistory
            {
                AccountID = recipientAccount.ID,
                Amount = amount,
                TransactionType = TransactionType.Transfer,
                TransactionDate = DateTime.Now,
                Description = description
            };
            await _transactionHistoryDal.InsertAsync(transaction);

            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "Transfer işlemi başarılı.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Transfer işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TDeleteAsync(int id)
    {
        try
        {
            await _transactionHistoryDal.DeleteAsync(id);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "İşlem başarıyla silindi.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("İşlem silme başarısız oldu. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetAllAsync()
    {
        try
        {
            var transactions = await _transactionHistoryDal.GetAllAsync();
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("İşlem geçmişi alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetByIdAsync(int id)
    {
        try
        {
            var transaction = await _transactionHistoryDal.GetByIdAsync(id);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transaction;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("İşlem alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TInsertAsync(TransactionHistory entity)
    {
        try
        {
            await _transactionHistoryDal.InsertAsync(entity);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "İşlem başarıyla eklendi.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("İşlem ekleme başarısız oldu. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TUpdateAsync(TransactionHistory entity)
    {
        try
        {
            await _transactionHistoryDal.UpdateAsync(entity);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "İşlem başarıyla güncellendi.";
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("İşlem güncelleme başarısız oldu. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetTotalAmountLast24HoursAsync(int accountId)
    {
        try
        {
            var totalAmount = await _transactionHistoryDal.GetTotalAmountLast24HoursAsync(accountId);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = totalAmount;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Son 24 saatteki toplam tutar alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetLast5TransfersUsersAsync(int accountId)
    {
        try
        {
            var transfers = await _transactionHistoryDal.GetLast5TransfersUsersAsync(accountId);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transfers;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Son 5 transfer alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetPagedTransactionHistoryAsync(int accountId, int page, int pageSize)
    {
        try
        {
            var transactions = await _transactionHistoryDal.GetPagedTransactionHistoryAsync(accountId, page, pageSize);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("İşlem geçmişi alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetTransactionVolumeLast24Hours()
    {
        try
        {
            var volume = await _transactionHistoryDal.GetTransactionVolumeLast24Hours();
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = volume;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Son 24 saatteki işlem hacmi alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetLastFiveTransactionsAsync()
    {
        try
        {
            var transactions = await _transactionHistoryDal.GetLastFiveTransactionsAsync();
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Son beş işlem alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetAdminPagedTransactionHistoryAsync(int page, int pageSize)
    {
        try
        {
            var transactions = await _transactionHistoryDal.GetAdminPagedTransactionHistoryAsync(page, pageSize);
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Yönetici işlem geçmişi alınamadı. Lütfen tekrar deneyin.");
        }
        return _response;
    }

    public async Task<BaseResponse> TGetLast5TransfersAllUsersAsync()
    {
        try
        {
            var transfers = await _transactionHistoryDal.GetLast5TransfersUsersAsync();
            _response.isSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transfers;
        }
        catch (Exception)
        {
            _response.isSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add("Son 5 transfer işlemi alınamadı. Lütfen tekrar deneyin.");
        }

        return _response;
    }

}








