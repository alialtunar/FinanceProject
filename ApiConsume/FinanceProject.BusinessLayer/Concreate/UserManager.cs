using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate.Jwt;
using FinanceProject.Core.Exceptions;
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
            try
            {
                userRegisterDto.FullName = userRegisterDto.FullName.ToLower();
                await _userDal.RegisterAsync(userRegisterDto);

                var newUser = await _userDal.ValidateUserAsync(new UserLoginDto
                {
                    Email = userRegisterDto.Email,
                    Password = userRegisterDto.Password
                });

                // Kullanıcıya ait bir account oluştur (0 bakiye ile)
                await _accountService.TInsertForUserAsync(newUser.ID);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcı kaydedilemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task<User> TLoginAsync(UserLoginDto userLoginDto)
        {
            try
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
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcı giriş yapamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task TDeleteAsync(int id)
        {
            try
            {
                await _userDal.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcı silinemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task<List<User>> TGetAllAsync()
        {
            try
            {
                return await _userDal.GetAllAsync();
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcılar alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task<User> TGetByIdAsync(int id)
        {
            try
            {
                return await _userDal.GetByIdAsync(id);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcı alınamadı. Lütfen tekrar deneyin.");
            }
        }

        public async Task TInsertAsync(User entity)
        {
            try
            {
                await _userDal.InsertAsync(entity);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcı eklenemedi. Lütfen tekrar deneyin.");
            }
        }

        public async Task TUpdateAsync(User entity)
        {
            try
            {
                await _userDal.UpdateAsync(entity);
            }
            catch (Exception)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, "Kullanıcı güncellenemedi. Lütfen tekrar deneyin.");
            }
        }
    }
}
