using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.EntityLayer.Concreate;
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
            await _accountDal.DeleteAsync(id);
        }

        public async Task<List<Account>> TGetAllAsync()
        {
            return await _accountDal.GetAllAsync();
        }

        public async Task<Account> TGetByIdAsync(int id)
        {
            return await _accountDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(Account entity)
        {
            await _accountDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(Account entity)
        {
            await _accountDal.UpdateAsync(entity);
        }

        public async Task TInsertForUserAsync(int userId)
        {
            // Benzersiz hesap numarası oluşturma
            string accountNumber = GenerateUniqueAccountNumber();

            // Yeni hesap oluşturma
            var newAccount = new Account
            {
                AccountNumber = accountNumber,
                Balance = 0,
                UserID = userId
            };

            // Hesabı veritabanına ekleme
            await _accountDal.InsertAsync(newAccount);
        }


        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            return await _accountDal.GetByAccountNumberAsync(accountNumber);
        }


        private string GenerateUniqueAccountNumber()
        {
            // Burada benzersiz bir hesap numarası oluşturma mantığını uygulayabilirsiniz
            // Örneğin, rastgele bir şekilde oluşturulabilir veya veritabanında mevcut
            // olmayan bir numara oluşturulabilir.

            // Örnek olarak bir rastgele numara oluşturma:
            Random random = new Random();
            string accountNumber = "ACC" + random.Next(100000, 999999).ToString();

            // Benzersiz olup olmadığını kontrol edebilirsiniz
            // Örneğin, _accountDal içindeki GetAllAsync() veya benzeri bir metotla kontrol edebilirsiniz

            return accountNumber;
        }
    }
}
