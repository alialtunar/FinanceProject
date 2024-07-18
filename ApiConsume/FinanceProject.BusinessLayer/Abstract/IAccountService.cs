using FinanceProject.EntityLayer.Concreate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IAccountService : IGenericService<Account>
    {
        Task TInsertForUserAsync(int userId);
        Task<Account> GetByAccountNumberAsync(string accountNumber);
    }
}
