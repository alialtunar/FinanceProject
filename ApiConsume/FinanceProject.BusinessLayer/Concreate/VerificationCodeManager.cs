using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
using System;
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

        public async Task<bool> VerifyCodeAsync(int accountId, string code, decimal amount, TransactionType transactionType)
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

        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task TDeleteAsync(int id)
        {
            await _verificationCodeDal.DeleteAsync(id);
        }

        public async Task<List<VerificationCode>> TGetAllAsync()
        {
            return await _verificationCodeDal.GetAllAsync();
        }

        public async Task<VerificationCode> TGetByIdAsync(int id)
        {
            return await _verificationCodeDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(VerificationCode entity)
        {
            await _verificationCodeDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(VerificationCode entity)
        {
            await _verificationCodeDal.UpdateAsync(entity);
        }
    }
}
