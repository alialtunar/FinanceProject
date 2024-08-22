using FinanceProject.Application.Models;
using FinanceProject.ApplicationLayer.Dtos.AccountDto;
using FinanceProject.EntityLayer.Concreate;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IAccountService
    {

        public Task<BaseResponse> TGetByAccountNumberAsync(string accountNumber);

        public Task<BaseResponse> TGetAccountByUserId(int userId);

        public Task<BaseResponse> TGetAccountDetailsAsync(int accountId);

        public Task<BaseResponse> TGetAdminPagedAccountsAsync(int page, int pageSize);

        public Task<BaseResponse> TDeleteAsync(int id);


        public Task<BaseResponse> TGetAllAsync();


        public Task<BaseResponse> TGetByIdAsync(int id);


        public Task<BaseResponse> TInsertAsync(Account entity);


        public Task<BaseResponse> TUpdateAsync(Account entity);

        public  Task<BaseResponse> TInsertForUserAsync(int userId);


    }
}
