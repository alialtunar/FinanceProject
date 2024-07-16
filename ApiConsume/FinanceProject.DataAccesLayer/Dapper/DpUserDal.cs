using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.DtoLayer.Dtos.UserDto;
using FinanceProject.EntityLayer.Concreate;
using System;
using System.Data;
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

        public async Task<User> ValidateUserAsync(UserLoginDto userLoginDto)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            var user = await _connection.QueryFirstOrDefaultAsync<User>(sql, new { userLoginDto.Email });

            if (user == null)
                return null;

            var storedSalt = Convert.FromBase64String(user.PasswordSalt);
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLoginDto.Password));
            var storedHash = Convert.FromBase64String(user.Password);

            if (!computedHash.SequenceEqual(storedHash))
                return null;

            return user;
        }
    }
}
