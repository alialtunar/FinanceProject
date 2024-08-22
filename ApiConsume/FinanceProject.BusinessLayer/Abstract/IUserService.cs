using FinanceProject.Application.Models;
using FinanceProject.ApplicationLayer.Dtos.UserDto;
using FinanceProject.ApplicationLayer.Exceptions;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IUserService 
    {
        Task<BaseResponse> TRegisterAsync(UserRegisterDto userRegisterDto);

        Task<BaseResponse> TLoginAsync(UserLoginDto userLoginDto);

        Task<BaseResponse> TGetTotalUserCount();

        Task<BaseResponse> TGetAdminPagedUsersAsync(int page, int pageSize);

         Task<BaseResponse> TDeleteAsync(int id);


        Task<BaseResponse> TGetAllAsync();



        Task<BaseResponse> TGetByIdAsync(int id);


        Task<BaseResponse> TInsertAsync(User entity);


        Task<BaseResponse> TUpdateAsync(User entity);
       

    }
}
