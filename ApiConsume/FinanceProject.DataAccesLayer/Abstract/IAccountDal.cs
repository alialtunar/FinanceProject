using FinanceProject.ApplicationLayer.Dtos.AccountDto;
using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface IAccountDal : IGenericDal<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);

        Task<Account> GetAccountByUserId(int userId);

        Task<IEnumerable<AccountWithUserDto>> GetAdminPagedAccountsAsync(int page, int pageSize);


    }
}
