using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate.Jwt;
using FinanceProject.ApplicationLayer.Exceptions;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.ApplicationLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FinanceProject.Application.Models;

namespace FinanceProject.BusinessLayer.Concreate
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly JwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly BaseResponse _response;

        public UserManager(IUserDal userDal, JwtService jwtService, IHttpContextAccessor httpContextAccessor, IAccountService accountService, BaseResponse response)
        {
            _userDal = userDal;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _response = response;
        }

        public async Task<BaseResponse> TRegisterAsync(UserRegisterDto userRegisterDto)
        {
            var response = _response;
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
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Kullanıcı doğrulama başarısız oldu.");
                    return response;
                }

                // Kullanıcıya ait bir account oluştur (0 bakiye ile)
                await _accountService.TInsertForUserAsync(newUser.ID);

                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Kullanıcı başarıyla kaydedildi.";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcı kaydedilemedi. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TLoginAsync(UserLoginDto userLoginDto)
        {
            var response = _response;
            try
            {
                var user = await _userDal.ValidateUserAsync(userLoginDto);
                if (user == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.ErrorMessages.Add("Kullanıcı bulunamadı.");
                    return response;
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

                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = user;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcı giriş yapamadı. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TDeleteAsync(int id)
        {
            var response = _response;
            try
            {
                await _userDal.DeleteAsync(id);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Kullanıcı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcı silinemedi. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TGetAllAsync()
        {
            var response = _response;
            try
            {
                var users = await _userDal.GetAllAsync();
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = users;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcılar alınamadı. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TGetByIdAsync(int id)
        {
            var response = _response;
            try
            {
                var user = await _userDal.GetByIdAsync(id);
                if (user == null)
                {
                    response.isSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Kullanıcı bulunamadı.");
                }
                else
                {
                    response.isSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = user;
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcı alınamadı. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TInsertAsync(User entity)
        {
            var response = _response;
            try
            {
                await _userDal.InsertAsync(entity);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Kullanıcı başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcı eklenemedi. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TUpdateAsync(User entity)
        {
            var response = _response;
            try
            {
                await _userDal.UpdateAsync(entity);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Kullanıcı başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcı güncellenemedi. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TGetTotalUserCount()
        {
            var response = _response;
            try
            {
                var totalCount = await _userDal.GetTotalUserCount();
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = totalCount;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Toplam kullanıcı sayısı alınamadı. Hata: {ex.Message}");
            }
            return response;
        }

        public async Task<BaseResponse> TGetAdminPagedUsersAsync(int page, int pageSize)
        {
            var response = _response;
            try
            {
                var users = await _userDal.GetAdminPagedUsersAsync(page, pageSize);
                response.isSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = users;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add($"Kullanıcılar alınamadı. Hata: {ex.Message}");
            }
            return response;
        }
    }
}
