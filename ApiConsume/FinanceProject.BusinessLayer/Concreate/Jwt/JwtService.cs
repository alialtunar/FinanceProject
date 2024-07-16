using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinanceProject.BusinessLayer.Concreate.Jwt
{
    public class JwtService
    {
        private readonly string _key;

        public JwtService(IConfiguration configuration)
        {
            _key = configuration["JwtSettings:SecretKey"];

            // Anahtar uzunluğunu kontrol et
            if (string.IsNullOrEmpty(_key) || _key.Length < 16)
            {
                throw new ArgumentException("Secret key must be at least 16 characters long.");
            }
        }

        // JWT oluşturma
        public string GenerateToken(string userId, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, role) // Kullanıcının rolü JWT içine ekleniyor
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token'ın geçerlilik süresi
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // Oluşturulan token'ı string olarak döndür
        }
    }
}
