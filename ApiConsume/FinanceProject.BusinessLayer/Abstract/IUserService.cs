using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Abstract
{
    public interface IUserService : IGenericService<User>
    {
        Task TRegisterAsync(UserRegisterDto userRegisterDto);

        Task<User> TLoginAsync(UserLoginDto userLoginDto);
    }
}
