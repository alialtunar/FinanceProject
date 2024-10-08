﻿using Dapper;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Repository;
using FinanceProject.EntityLayer.Concreate;
using System.Data;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Dapper
{
    public class DpVerificationCodeDal : GenericRepository<VerificationCode>, IVerificationCodeDal
    {
        private readonly IDbConnection _connection;

        public DpVerificationCodeDal(IDbConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<VerificationCode>> GetAdminPagedVerificationCodesAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var sql = @"
        SELECT ID, AccountId, Code, Amount, TransactionType, ExpirationTime, IsUsed
        FROM VerificationCodes
        ORDER BY ID ASC
        LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                PageSize = pageSize,
                Offset = offset
            };

            return await _connection.QueryAsync<VerificationCode>(sql, parameters);
        }


        public async Task<VerificationCode> GetByCodeAsync(string code)
        {
            try
            {
                var sql = "SELECT * FROM verificationcode WHERE code = @Code";
                return await _connection.QuerySingleOrDefaultAsync<VerificationCode>(sql, new { Code = code });
            }
            catch (Exception ex)
            {
                throw new Exception($"Kod ile doğrulama kodu alınamadı. Hata: {ex.Message}");
            }
        }

        public async Task UpdateAsync(VerificationCode verificationCode)
        {
            try
            {
                var sql = "UPDATE verificationcode SET \"isused\" = @IsUsed WHERE \"id\" = @Id";
                await _connection.ExecuteAsync(sql, verificationCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Doğrulama kodu güncellenemedi. Hata: {ex.Message}");
            }
        }
    }
}
