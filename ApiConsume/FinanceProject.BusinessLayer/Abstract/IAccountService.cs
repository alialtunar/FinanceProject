using FinanceProject.DtoLayer.Dtos.AccountDto;
using FinanceProject.EntityLayer.Concreate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IAccountService : IGenericService<Account>
    {
        Task TInsertForUserAsync(int userId);
        Task<Account> TGetByAccountNumberAsync(string accountNumber);

        Task<Account> TGetAccountByUserId(int userId);

        Task<AccountDetailsDto> TGetAccountDetailsAsync(int accountId);
    }
}
