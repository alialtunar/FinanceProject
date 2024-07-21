using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using FinanceProject.Core.Exceptions;
using Microsoft.AspNetCore.Http;
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
            try
            {
                var values = await _userService.TGetAllAsync();
                return Ok(values);
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcılar alınamadı. Lütfen tekrar deneyin.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            try
            {
                await _userService.TInsertAsync(user);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı eklenemedi. Lütfen tekrar deneyin.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.TDeleteAsync(id);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı silinemedi. Lütfen tekrar deneyin.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            try
            {
                await _userService.TUpdateAsync(user);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı güncellenemedi. Lütfen tekrar deneyin.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var value = await _userService.TGetByIdAsync(id);
                return Ok(value);
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı alınamadı. Lütfen tekrar deneyin.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            try
            {
                await _userService.TRegisterAsync(userRegisterDto);
                return Ok();
            }
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı kaydedilemedi. Lütfen tekrar deneyin.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
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
            catch (ErrorException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı giriş yapamadı. Lütfen tekrar deneyin.");
            }
        }
    }
}
