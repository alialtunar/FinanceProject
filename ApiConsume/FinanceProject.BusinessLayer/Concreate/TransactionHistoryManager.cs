using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.Core.Exceptions;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DtoLayer.Dtos.TransactionHistoryDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;

public class TransactionHistoryManager : ITransactionHistoryService
{
    private readonly ITransactionHistoryDal _transactionHistoryDal;
    private readonly IAccountService _accountService;
    private readonly IVerificationCodeService _verificationCodeService;
    private readonly IUserService _userService;

    public TransactionHistoryManager(ITransactionHistoryDal transactionHistoryDal, IAccountService accountService, IVerificationCodeService verificationCodeService, IUserService userService)
    {
        _transactionHistoryDal = transactionHistoryDal;
        _accountService = accountService;
        _verificationCodeService = verificationCodeService;
        _userService = userService;
    }

    public async Task<List<TransactionHistory>> TGetLastFiveTransactionsAsync(int accountId)
    {
        return await _transactionHistoryDal.GetLastFiveTransactionsAsync(accountId);
    }


    public async Task<string> InitiateDeposit(int accountId, decimal amount, string description = null)
    {
        try
        {
            var verificationCode = await _verificationCodeService.CreateVerificationCodeAsync(accountId, amount, TransactionType.Deposit);
            return verificationCode.Code;
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "Para yatırma işlemi başlatılamadı. Lütfen tekrar deneyin.");
        }
    }

    public async Task<string> InitiateWithdraw(int accountId, decimal amount, string description = null)
    {
        try
        {
            var account = await _accountService.TGetByIdAsync(accountId);
            if (account.Balance < amount)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, "Yetersiz bakiye.");
            }
            var verificationCode = await _verificationCodeService.CreateVerificationCodeAsync(accountId, amount, TransactionType.Withdrawal);
            return verificationCode.Code;
        }
        catch (ErrorException ex)
        {
            throw new ErrorException(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "Para çekme işlemi başlatılamadı. Lütfen tekrar deneyin.");
        }
    }

    public async Task Deposit(int accountId, decimal amount, string verificationCode, string description = null)
    {
        try
        {
            if (!await _verificationCodeService.VerifyCodeAsync(accountId, verificationCode, amount, TransactionType.Deposit))
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, "Geçersiz veya süresi dolmuş doğrulama kodu.");
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
        catch (ErrorException ex)
        {
            throw new ErrorException(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "Para yatırma işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    public async Task Withdraw(int accountId, decimal amount, string verificationCode, string description = null)
    {
        try
        {
            if (!await _verificationCodeService.VerifyCodeAsync(accountId, verificationCode, amount, TransactionType.Withdrawal))
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, "Geçersiz veya süresi dolmuş doğrulama kodu.");
            }

            var account = await _accountService.TGetByIdAsync(accountId);
            if (account.Balance < amount)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, "Yetersiz bakiye.");
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
        catch (ErrorException ex)
        {
            throw new ErrorException(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "Para çekme işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    public async Task InitiateTransfer(int senderAccountId, string recipientAccountNumber, decimal amount)
    {
        try
        {
            var senderAccount = await _accountService.TGetByIdAsync(senderAccountId);
            if (senderAccount.Balance < amount)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, "Yetersiz bakiye.");
            }

            var recipientAccount = await _accountService.TGetByAccountNumberAsync(recipientAccountNumber);
            if (recipientAccount == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, "Alıcı bulunamadı.");
            }
        }
        catch (ErrorException ex)
        {
            throw new ErrorException(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "Havale işlemi başlatılamadı. Lütfen tekrar deneyin.");
        }
    }

    public async Task Transfer(int senderAccountId, string recipientAccountNumber, decimal amount, string recipientName, string description = null)
    {
        try
        {
            var senderAccount = await _accountService.TGetByIdAsync(senderAccountId);
            if (senderAccount.Balance < amount)
            {
                throw new ErrorException(StatusCodes.Status403Forbidden, "Yetersiz bakiye.");
            }

            var recipientAccount = await _accountService.TGetByAccountNumberAsync(recipientAccountNumber);
            if (recipientAccount == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, "Alıcı bulunamadı.");
            }

            var recipientUser = await _userService.TGetByIdAsync(recipientAccount.UserID);
            var fullName = recipientUser.FullName.ToLower();
            if (fullName != recipientName.ToLower())
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, "Alıcı bilgileri uyuşmuyor.");
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
                RecipientAccountNumber = recipientAccountNumber,
                RecipientName = recipientName,
                Description = description
            };
            await _transactionHistoryDal.InsertAsync(transaction);
        }
        catch (ErrorException ex)
        {
            throw new ErrorException(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "Havale işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    public async Task TDeleteAsync(int id)
    {
        try
        {
            await _transactionHistoryDal.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "İşlem silme başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    public async Task<List<TransactionHistory>> TGetAllAsync()
    {
        try
        {
            return await _transactionHistoryDal.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "İşlem geçmişi alınamadı. Lütfen tekrar deneyin.");
        }
    }

    public async Task<TransactionHistory> TGetByIdAsync(int id)
    {
        try
        {
            return await _transactionHistoryDal.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "İşlem alınamadı. Lütfen tekrar deneyin.");
        }
    }

    public async Task TInsertAsync(TransactionHistory entity)
    {
        try
        {
            await _transactionHistoryDal.InsertAsync(entity);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "İşlem ekleme başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    public async Task TUpdateAsync(TransactionHistory entity)
    {
        try
        {
            await _transactionHistoryDal.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            throw new ErrorException(StatusCodes.Status500InternalServerError, "İşlem güncelleme başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    public async Task<decimal> TGetTotalAmountLast24HoursAsync(int accountId)
    {
        return await _transactionHistoryDal.GetTotalAmountLast24HoursAsync(accountId);
    }

    public async Task<IEnumerable<LastTransfersDto>> TGetLast5TransfersUsersAsync(int accountId)
    {
        return await _transactionHistoryDal.GetLast5TransfersUsersAsync(accountId);
    }

    public async Task<IEnumerable<TransactionHistory>> TGetPagedTransactionHistoryAsync(int accountId, int page, int pageSize)
    {
        return await _transactionHistoryDal.GetPagedTransactionHistoryAsync(accountId, page, pageSize);
    }
}
