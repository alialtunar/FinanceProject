using FinanceProject.ApplicationLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface IUserDal : IGenericDal<User>
    {
        Task RegisterAsync(UserRegisterDto userRegisterDto);

        Task<User> ValidateUserAsync(UserLoginDto userLoginDto);

        Task<int> GetTotalUserCount();

        Task<IEnumerable<User>> GetAdminPagedUsersAsync(int page, int pageSize);
    }
}
