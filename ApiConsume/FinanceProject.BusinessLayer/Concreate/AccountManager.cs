using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.Core.Exceptions;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Dapper;
using FinanceProject.DtoLayer.Dtos.AccountDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class AccountManager : IAccountService
    {
        private readonly IAccountDal _accountDal;
        private readonly IUserDal _userDal;


        public AccountManager(IAccountDal accountDal, IUserDal userDal)
        {
            _accountDal = accountDal;
            _userDal = userDal;
        }

        public async Task TDeleteAsync(int id)
        {
            try
            {
                await _accountDal.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesap silinemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task<List<Account>> TGetAllAsync()
        {
            try
            {
                return await _accountDal.GetAllAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesaplar alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task<Account> TGetByIdAsync(int id)
        {
            try
            {
                return await _accountDal.GetByIdAsync(id);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesap alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task TInsertAsync(Account entity)
        {
            try
            {
                await _accountDal.InsertAsync(entity);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesap eklenemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task TUpdateAsync(Account entity)
        {
            try
            {
                await _accountDal.UpdateAsync(entity);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesap güncellenemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task TInsertForUserAsync(int userId)
        {
            try
            {
                var newAccountNumber = GenerateUniqueAccountNumber();
                var newAccount = new Account
                {
                    UserID = userId,
                    AccountNumber = newAccountNumber,
                    Balance = 0
                };
                await _accountDal.InsertAsync(newAccount);
              
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesap oluşturulamadı. Lütfen tekrar deneyin.");
            }
        }


        public async Task<Account> TGetByAccountNumberAsync(string accountNumber)
        {
            try
            {
                return await _accountDal.GetByAccountNumberAsync(accountNumber);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Hesap numarasına göre hesap alınamadı. Lütfen tekrar deneyin.");
            }
        }

       

        private string GenerateUniqueAccountNumber()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
        }

        public async Task<Account> TGetAccountByUserId(int userId)
        {
            try
            {
                return await _accountDal.GetAccountByUserId(userId);
            }
            catch(Exception ex) 
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "User id'ye göre hesap alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task<AccountDetailsDto> TGetAccountDetailsAsync(int accountId)
        {
            var account = await _accountDal.GetByIdAsync(accountId);
            if (account == null) return null;

            var user = await _userDal.GetByIdAsync(account.UserID);
            if (user == null) return null;

            return new AccountDetailsDto
            {
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                FullName = user.FullName
            };
        }
    }
}
