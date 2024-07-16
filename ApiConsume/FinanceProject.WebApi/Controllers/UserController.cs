﻿using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public UserController(IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult UserList()
        {
            var values = _userService.TGetAll();
            return Ok(values);
        }

        [HttpPost]

         public IActionResult AddUser(User user)
        {
            _userService.TInsert(user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var value = _userService.TGetById(id);
            _userService.TDelete(value.ID);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdateUser(User user)
        {
            _userService.TUpdate(user);
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var value = _userService.TGetById(id);
            return Ok(value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            await _userService.TRegister(userRegisterDto);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _userService.TLogin(userLoginDto);
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
