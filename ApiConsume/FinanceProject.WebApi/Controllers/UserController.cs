using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.ApplicationLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FinanceProject.Application.Models;

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
        public async Task<ActionResult> UserList()
        {
            var response = await _userService.TGetAllAsync();
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpGet("Admin/paged")]
        public async Task<ActionResult> GetPagedUsers(int page = 1, int pageSize = 6)
        {
            var response = await _userService.TGetAdminPagedUsersAsync(page, pageSize);
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody] User user)
        {
            var response = await _userService.TInsertAsync(user);
            if (!response.isSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var response = await _userService.TDeleteAsync(id);
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] User user)
        {
            var response = await _userService.TUpdateAsync(user);
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var response = await _userService.TGetByIdAsync(id);
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var response = await _userService.TRegisterAsync(userRegisterDto);
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var response = await _userService.TLoginAsync(userLoginDto);
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            var user = response.Result as User;

            return Ok(new BaseResponse
            {
                isSuccess = true,
                Result = new
                {
                    user.ID,
                    user.Email,
                    user.FullName,
                    user.Phone,
                    user.Role
                }
            });
        }

        [HttpGet("user-count")]
        public async Task<ActionResult> GetTotalUserCount()
        {
            var response = await _userService.TGetTotalUserCount();
            if (!response.isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }
    }
}
