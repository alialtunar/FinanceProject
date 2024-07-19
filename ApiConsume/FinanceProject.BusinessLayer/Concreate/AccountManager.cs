using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.Core.Exceptions;
using FinanceProject.DataAccesLayer.Abstract;
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

        public AccountManager(IAccountDal accountDal)
        {
            _accountDal = accountDal;
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


        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
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
    }
}
