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

                if (newUser == null)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, "Kullanıcı doğrulama başarısız oldu.");
                }

                // Kullanıcıya ait bir account oluştur (0 bakiye ile)
                await _accountService.TInsertForUserAsync(newUser.ID);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcı kaydedilemedi. Hata: {ex.Message}");
            }
        }

        public async Task<User> TLoginAsync(UserLoginDto userLoginDto)
        {
            try
            {
                var user = await _userDal.ValidateUserAsync(userLoginDto);
                if (user == null)
                {
                    throw new ErrorException(StatusCodes.Status403Forbidden, "Kullanıcı bulunamadı.");
                }

                var token = _jwtService.GenerateToken(user.ID.ToString(), user.Role);

                // Token'i HTTP response içinde cookie olarak gönder
                var httpResponse = _httpContextAccessor.HttpContext.Response;
                httpResponse.Cookies.Append("JWTToken", token, new CookieOptions
                {
                    HttpOnly = false, // JavaScript üzerinden erişilebilir
                    Secure = true,   // HTTPS üzerinden iletişimde kullanılabilir (gerektiğinde)
                    SameSite = SameSiteMode.None, // Güvenlik için SameSite ayarı
                    Expires = DateTime.UtcNow.AddDays(2) // Token'ın geçerlilik süresi
                });

                return user;
            }
            catch (ErrorException ex)
            {
                throw new ErrorException(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcı giriş yapamadı. Hata: {ex.Message}");
            }
        }

        public async Task TDeleteAsync(int id)
        {
            try
            {
                await _userDal.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcı silinemedi. Hata: {ex.Message}");
            }
        }

        public async Task<List<User>> TGetAllAsync()
        {
            try
            {
                return await _userDal.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcılar alınamadı. Hata: {ex.Message}");
            }
        }

        public async Task<User> TGetByIdAsync(int id)
        {
            try
            {
                return await _userDal.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcı alınamadı. Hata: {ex.Message}");
            }
        }

        public async Task TInsertAsync(User entity)
        {
            try
            {
                await _userDal.InsertAsync(entity);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcı eklenemedi. Hata: {ex.Message}");
            }
        }

        public async Task TUpdateAsync(User entity)
        {
            try
            {
                await _userDal.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, $"Kullanıcı güncellenemedi. Hata: {ex.Message}");
            }
        }
    }
}
