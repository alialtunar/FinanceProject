using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinanceProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var values = await _userService.TGetAllAsync();
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            await _userService.TInsertAsync(user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.TDeleteAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            await _userService.TUpdateAsync(user);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var value = await _userService.TGetByIdAsync(id);
            return Ok(value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            await _userService.TRegisterAsync(userRegisterDto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _userService.TLoginAsync(userLoginDto);
            if (user == null) return Unauthorized();

            // Kullanıcı bilgilerini döndür
            return Ok(new
            {
                user.ID,
                user.Email,
                user.FullName,
                user.Phone,
                user.Role
            });
        }

    }
}
