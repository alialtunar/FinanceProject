using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.ApplicationLayer.Exceptions;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class VerificationCodeManager : IVerificationCodeService
    {
        private readonly IVerificationCodeDal _verificationCodeDal;

        public VerificationCodeManager(IVerificationCodeDal verificationCodeDal)
        {
            _verificationCodeDal = verificationCodeDal;
        }

        public async Task<VerificationCode> CreateVerificationCodeAsync(int accountId, decimal amount, TransactionType transactionType)
        {
            try
            {
                var code = GenerateCode();
                var verificationCode = new VerificationCode
                {
                    AccountId = accountId,
                    Code = code,
                    Amount = amount,
                    TransactionType = transactionType,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(2),
                    IsUsed = false
                };
                await _verificationCodeDal.InsertAsync(verificationCode);
                return verificationCode;
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodu oluşturulamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task<bool> VerifyCodeAsync(int accountId, string code, decimal amount, TransactionType transactionType)
        {
            try
            {
                var verificationCode = await _verificationCodeDal.GetByCodeAsync(code);
                if (verificationCode == null || verificationCode.IsUsed || verificationCode.ExpirationTime < DateTime.UtcNow)
                {
                    return false;
                }
                if (verificationCode.AccountId != accountId || verificationCode.Amount != amount || verificationCode.TransactionType != transactionType)
                {
                    return false;
                }
                verificationCode.IsUsed = true;
                await _verificationCodeDal.UpdateAsync(verificationCode);
                return true;
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodu doğrulanamadı. Lütfen tekrar deneyin.");
            }
        }

        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task TDeleteAsync(int id)
        {
            try
            {
                await _verificationCodeDal.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodu silinemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task<List<VerificationCode>> TGetAllAsync()
        {
            try
            {
                return await _verificationCodeDal.GetAllAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodları alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task<VerificationCode> TGetByIdAsync(int id)
        {
            try
            {
                return await _verificationCodeDal.GetByIdAsync(id);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodu alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task TInsertAsync(VerificationCode entity)
        {
            try
            {
                await _verificationCodeDal.InsertAsync(entity);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodu eklenemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task TUpdateAsync(VerificationCode entity)
        {
            try
            {
                await _verificationCodeDal.UpdateAsync(entity);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Doğrulama kodu güncellenemedi. Lütfen tekrar deneyin.");
            }
        }
    }
}
