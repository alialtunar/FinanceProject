using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface IUserDal : IGenericDal<User>
    {
        Task RegisterAsync(UserRegisterDto userRegisterDto);

        Task<User> ValidateUserAsync(UserLoginDto userLoginDto);
    }
}
