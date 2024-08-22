using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Dapper;
using FinanceProject.ApplicationLayer.Dtos.AccountDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FinanceProject.Application.Models;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class AccountManager : IAccountService
    {
        private readonly IAccountDal _accountDal;
        private readonly IUserDal _userDal;
        private readonly BaseResponse _response;

        // Constructor now takes BaseResponse as a parameter
        public AccountManager(IAccountDal accountDal, IUserDal userDal, BaseResponse response)
        {
            _accountDal = accountDal;
            _userDal = userDal;
            _response = response;
        }

        public async Task<BaseResponse> TDeleteAsync(int id)
        {
            var response = _response;
            try
            {
                await _accountDal.DeleteAsync(id);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Hesap başarıyla silindi.";
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap silinemedi. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TGetAllAsync()
        {
            var response = _response;
            try
            {
                var accounts = await _accountDal.GetAllAsync();
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = accounts;
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesaplar alınamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TGetByIdAsync(int id)
        {
            var response = _response;
            try
            {
                var account = await _accountDal.GetByIdAsync(id);
                if (account == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Hesap bulunamadı.");
                }
                else
                {
                    response.isSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = account;
                }
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap alınamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TInsertAsync(Account entity)
        {
            var response = _response;
            try
            {
                await _accountDal.InsertAsync(entity);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Hesap başarıyla eklendi.";
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap eklenemedi. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TUpdateAsync(Account entity)
        {
            var response = _response;
            try
            {
                await _accountDal.UpdateAsync(entity);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Hesap başarıyla güncellendi.";
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap güncellenemedi. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TGetByAccountNumberAsync(string accountNumber)
        {
            var response = _response;
            try
            {
                var account = await _accountDal.GetByAccountNumberAsync(accountNumber);
                if (account == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Hesap numarasına göre hesap bulunamadı.");
                }
                else
                {
                    response.isSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = account;
                }
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap numarasına göre hesap alınamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TGetAccountByUserId(int userId)
        {
            var response = _response;
            try
            {
                var account = await _accountDal.GetAccountByUserId(userId);
                if (account == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Kullanıcı id'sine göre hesap bulunamadı.");
                }
                else
                {
                    response.isSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = account;
                }
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Kullanıcı id'sine göre hesap alınamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TGetAccountDetailsAsync(int accountId)
        {
            var response = _response;
            try
            {
                var account = await _accountDal.GetByIdAsync(accountId);
                if (account == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Hesap bulunamadı.");
                    return response;
                }

                var user = await _userDal.GetByIdAsync(account.UserID);
                if (user == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Kullanıcı bulunamadı.");
                    return response;
                }

                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = new AccountDetailsDto
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance,
                    FullName = user.FullName
                };
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap detayları alınamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }

        public async Task<BaseResponse> TGetAdminPagedAccountsAsync(int page, int pageSize)
        {
            var response = _response;
            try
            {
                var accounts = await _accountDal.GetAdminPagedAccountsAsync(page, pageSize);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = accounts;
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesaplar alınamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }

        private string GenerateUniqueAccountNumber()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
        }

        public async Task<BaseResponse> TInsertForUserAsync(int userId)
        {
            var response = _response;
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
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Hesap başarıyla oluşturuldu.";
            }
            catch (Exception)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Hesap oluşturulamadı. Lütfen tekrar deneyin.");
            }
            return response;
        }
    }
}
