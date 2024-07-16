using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate.Jwt;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly JwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;

        public UserManager(IUserDal userDal, JwtService jwtService, IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _userDal = userDal;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        }

        public async Task TRegisterAsync(UserRegisterDto userRegisterDto)
        {
            await _userDal.RegisterAsync(userRegisterDto);

            var newUser = await _userDal.ValidateUserAsync(new UserLoginDto
            {
                Email = userRegisterDto.Email,
                Password = userRegisterDto.Password
            });

            // Kullanıcıya ait bir account oluştur (0 bakiye ile)
            await _accountService.TInsertForUserAsync(newUser.ID);
        }

        public async Task<User> TLoginAsync(UserLoginDto userLoginDto)
        {
            var user = await _userDal.ValidateUserAsync(userLoginDto);
            if (user == null) return null;

            var token = _jwtService.GenerateToken(user.ID.ToString(), user.Role);

            // Token'i HTTP response içinde cookie olarak gönder
            var httpResponse = _httpContextAccessor.HttpContext.Response;
            httpResponse.Cookies.Append("JWTToken", token, new CookieOptions
            {
                HttpOnly = true, // Sadece HTTP üzerinden erişilebilir
                Secure = true,   // HTTPS üzerinden iletişimde kullanılabilir (gerektiğinde)
                SameSite = SameSiteMode.Strict, // Güvenlik için SameSite ayarı
                Expires = DateTime.UtcNow.AddHours(1) // Token'ın geçerlilik süresi
            });

            return user;
        }

        public async Task TDeleteAsync(int id)
        {
            await _userDal.DeleteAsync(id);
        }

        public async Task<List<User>> TGetAllAsync()
        {
            return await _userDal.GetAllAsync();
        }

        public async Task<User> TGetByIdAsync(int id)
        {
            return await _userDal.GetByIdAsync(id);
        }

        public async Task TInsertAsync(User entity)
        {
            await _userDal.InsertAsync(entity);
        }

        public async Task TUpdateAsync(User entity)
        {
            await _userDal.UpdateAsync(entity);
        }
    }
}
