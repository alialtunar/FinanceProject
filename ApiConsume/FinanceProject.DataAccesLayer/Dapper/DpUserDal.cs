using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpUserDal : GenericRepository<User>, IUserDal
    {
        private readonly IDbConnection _connection;

        public DpUserDal(IDbConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public async Task RegisterAsync(UserRegisterDto userRegisterDto)
        {
            try
            {
                // E-posta adresinin var olup olmadığını kontrol et
                var existingUser = await GetByEmailAsync(userRegisterDto.Email);
                if (existingUser != null)
                {
                    throw new Exception("Bu e-posta adresi zaten mevcut.");
                }

                // Kullanıcı kaydetme işlemi
                using var hmac = new HMACSHA512();
                var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDto.Password)));
                var passwordSalt = Convert.ToBase64String(hmac.Key);

                var user = new User
                {
                    Email = userRegisterDto.Email,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = userRegisterDto.FullName,
                    Role = "user"
                };

                var sql = "INSERT INTO Users (Email, Password, PasswordSalt, FullName, Phone, Role) VALUES (@Email, @Password, @PasswordSalt, @FullName, @Phone, @Role)";
                await _connection.ExecuteAsync(sql, user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // E-posta adresine göre kullanıcıyı getiren bir yardımcı metod
        private async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE Email = @Email";
                return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
            }
            catch (Exception ex)
            {
                throw new Exception($"E-posta adresi kontrol edilemedi. Hata: {ex.Message}");
            }
        }


        public async Task<User> ValidateUserAsync(UserLoginDto userLoginDto)
        {
            try
            {
                var sql = "SELECT * FROM Users WHERE Email = @Email";
                var user = await _connection.QueryFirstOrDefaultAsync<User>(sql, new { userLoginDto.Email });

                if (user == null)
                {
                    throw new Exception("Kullanıcı bulunamadı.");
                }

                var storedSalt = Convert.FromBase64String(user.PasswordSalt);
                using var hmac = new HMACSHA512(storedSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLoginDto.Password));
                var storedHash = Convert.FromBase64String(user.Password);

                if (!computedHash.SequenceEqual(storedHash))
                {
                    throw new Exception("Şifre yanlış.");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalUserCount()
        {
            string query = "SELECT COUNT(*) FROM Users";
            return await _connection.QueryFirstOrDefaultAsync<int>(query);
        }
    }
}
